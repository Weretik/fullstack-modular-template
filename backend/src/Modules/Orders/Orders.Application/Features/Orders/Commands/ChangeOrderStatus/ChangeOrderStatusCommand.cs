using BuildingBlocks.Domain.Results;
using Mediator;
using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Commands.ChangeOrderStatus;

public sealed record ChangeOrderStatusCommand(Guid OrderId, OrderStatus Status) : ICommand<Result>;
