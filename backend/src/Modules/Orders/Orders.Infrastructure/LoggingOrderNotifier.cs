using Microsoft.Extensions.Logging;
using Orders.Application.Features.Notifications;
using Orders.Contracts;

namespace Orders.Infrastructure;

/// <summary>
/// The sample notifier writes to the log. Replace it with an e-mail or chat adapter; the outbox already retries
/// it, so the adapter does not need its own retry loop.
/// </summary>
internal sealed partial class LoggingOrderNotifier(ILogger<LoggingOrderNotifier> logger) : IOrderNotifier
{
    public Task NotifyStaffAsync(OrderPlaced order, CancellationToken cancellationToken)
    {
        LogNewOrder(logger, order.OrderId, order.CustomerName, order.Total);
        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "New order {OrderId} from {Customer} for {Total}")]
    private static partial void LogNewOrder(ILogger logger, Guid orderId, string customer, decimal total);
}
