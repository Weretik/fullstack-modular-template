using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Catalog.Contracts;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Orders.Contracts;
using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Commands.PlaceOrder;

/// <summary>
/// Places an order at the current catalog prices. Idempotent: the order and its <see cref="OrderPlaced"/>
/// message are saved in one transaction under a unique idempotency key, and a repeat returns the first order.
/// </summary>
public sealed class PlaceOrderHandler(IOrdersDbContext db, ICatalogProductReader catalog, TimeProvider clock)
    : ICommandHandler<PlaceOrderCommand, Result<PlacedOrderResponse>>
{
    public async ValueTask<Result<PlacedOrderResponse>> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var fingerprint = PlaceOrderFingerprint.Of(command);
        if (await FindRepeatAsync(command.IdempotencyKey, fingerprint, cancellationToken) is { } repeat)
            return repeat;

        var products = (await catalog.GetForSaleAsync([.. command.Lines.Select(l => l.ProductId)], cancellationToken))
            .ToDictionary(p => p.Id);
        var lines = new List<OrderLine>(command.Lines.Count);
        foreach (var requested in command.Lines)
        {
            if (!products.TryGetValue(requested.ProductId, out var product))
                return OrderErrors.ProductUnavailable;

            var line = OrderLine.Create(product.Id, product.Name, product.Price, requested.Quantity);
            if (line.IsFailure)
                return line.Error!;
            lines.Add(line.Value);
        }

        var order = Order.Place(command.CustomerName, command.Phone, lines, command.IdempotencyKey, fingerprint, clock.GetUtcNow());
        if (order.IsFailure)
            return order.Error!;

        db.Orders.Add(order.Value);
        db.AddOutboxMessage(new OrderPlaced(order.Value.Id, order.Value.CustomerName, order.Value.Total, order.Value.PlacedAtUtc));
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            // A parallel copy of this request saved first: answer with its order.
            return await FindRepeatAsync(command.IdempotencyKey, fingerprint, cancellationToken)
                ?? throw new InvalidOperationException("The idempotency key was taken, but no order holds it.");
        }

        return new PlacedOrderResponse(order.Value.Id, order.Value.Total);
    }

    private async Task<Result<PlacedOrderResponse>?> FindRepeatAsync(
        string idempotencyKey, string fingerprint, CancellationToken cancellationToken)
    {
        var existing = await db.Orders
            .AsNoTracking()
            .Where(o => o.IdempotencyKey == idempotencyKey)
            .Select(o => new { o.Id, o.Total, o.RequestHash })
            .FirstOrDefaultAsync(cancellationToken);
        if (existing is null)
            return null;

        return existing.RequestHash == fingerprint
            ? new PlacedOrderResponse(existing.Id, existing.Total)
            : OrderErrors.IdempotencyKeyReused;
    }
}
