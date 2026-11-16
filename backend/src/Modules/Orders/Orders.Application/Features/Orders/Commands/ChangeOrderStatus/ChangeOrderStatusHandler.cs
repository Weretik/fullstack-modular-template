using BuildingBlocks.Domain.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Commands.ChangeOrderStatus;

/// <summary>
/// Moves an order along its allowed statuses. Setting the status it already has succeeds, so a repeat is safe;
/// the row version turns a parallel change by another manager into 409 instead of a lost update.
/// </summary>
public sealed class ChangeOrderStatusHandler(IOrdersDbContext db) : ICommandHandler<ChangeOrderStatusCommand, Result>
{
    public async ValueTask<Result> Handle(ChangeOrderStatusCommand command, CancellationToken cancellationToken)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == command.OrderId, cancellationToken);
        if (order is null)
            return OrderErrors.NotFound;

        var changed = order.ChangeStatus(command.Status);
        if (changed.IsFailure)
            return changed;

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return OrderErrors.ChangedByOtherUser;
        }

        return Result.Success();
    }
}
