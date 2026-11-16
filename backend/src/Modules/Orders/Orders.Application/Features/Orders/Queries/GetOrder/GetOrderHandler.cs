using BuildingBlocks.Domain.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Queries.GetOrder;

public sealed class GetOrderHandler(IOrdersDbContext db) : IQueryHandler<GetOrderQuery, Result<OrderDetailsResponse>>
{
    public async ValueTask<Result<OrderDetailsResponse>> Handle(GetOrderQuery query, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == query.Id)
            .Select(o => new OrderDetailsResponse(
                o.Id, o.CustomerName, o.Phone, o.Status, o.Total, o.PlacedAtUtc,
                o.Lines.Select(l => new OrderLineResponse(l.ProductId, l.ProductName, l.UnitPrice, l.Quantity, l.UnitPrice * l.Quantity)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);
        return order is null ? OrderErrors.NotFound : order;
    }
}
