using BuildingBlocks.Domain.Results;
using Mediator;

namespace Orders.Application.Features.Orders.Commands.PlaceOrder;

/// <param name="IdempotencyKey">One key per checkout attempt, sent again on retry (the Idempotency-Key header).</param>
public sealed record PlaceOrderCommand(
    string IdempotencyKey,
    string CustomerName,
    string Phone,
    IReadOnlyList<PlaceOrderLine> Lines) : ICommand<Result<PlacedOrderResponse>>;
