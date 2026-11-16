using BuildingBlocks.Application.Outbox;
using Orders.Contracts;

namespace Orders.Application.Features.Notifications;

/// <summary>Runs from the outbox tick after an order is saved, so a slow notifier never fails or delays checkout.</summary>
public sealed class OrderPlacedHandler(IOrderNotifier notifier) : IOutboxMessageHandler<OrderPlaced>
{
    public Task HandleAsync(OrderPlaced message, CancellationToken cancellationToken) =>
        notifier.NotifyStaffAsync(message, cancellationToken);
}
