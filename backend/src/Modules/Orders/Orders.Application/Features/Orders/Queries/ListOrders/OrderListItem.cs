using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Queries.ListOrders;

public sealed record OrderListItem(Guid Id, string CustomerName, string Phone, OrderStatus Status, decimal Total, DateTimeOffset PlacedAtUtc);
