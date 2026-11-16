using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Queries.GetOrder;

public sealed record OrderDetailsResponse(
    Guid Id,
    string CustomerName,
    string Phone,
    OrderStatus Status,
    decimal Total,
    DateTimeOffset PlacedAtUtc,
    IReadOnlyList<OrderLineResponse> Lines);
