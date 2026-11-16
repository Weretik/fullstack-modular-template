namespace Orders.Application.Features.Orders.Queries.GetOrder;

public sealed record OrderLineResponse(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
