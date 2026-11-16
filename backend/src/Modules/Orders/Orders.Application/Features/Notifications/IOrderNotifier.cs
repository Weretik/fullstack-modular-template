using Orders.Contracts;

namespace Orders.Application.Features.Notifications;

/// <summary>Tells staff about a new order (e-mail, chat, ...). Infrastructure decides how.</summary>
public interface IOrderNotifier
{
    Task NotifyStaffAsync(OrderPlaced order, CancellationToken cancellationToken);
}
