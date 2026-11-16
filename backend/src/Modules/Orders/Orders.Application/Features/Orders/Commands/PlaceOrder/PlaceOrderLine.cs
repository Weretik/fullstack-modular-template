namespace Orders.Application.Features.Orders.Commands.PlaceOrder;

public sealed record PlaceOrderLine(Guid ProductId, int Quantity);
