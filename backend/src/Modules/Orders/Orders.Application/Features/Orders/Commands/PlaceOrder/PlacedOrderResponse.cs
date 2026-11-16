namespace Orders.Application.Features.Orders.Commands.PlaceOrder;

public sealed record PlacedOrderResponse(Guid OrderId, decimal Total);
