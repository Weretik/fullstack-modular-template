using BuildingBlocks.Domain.Results;
using Mediator;

namespace Orders.Application.Features.Orders.Queries.GetOrder;

public sealed record GetOrderQuery(Guid Id) : IQuery<Result<OrderDetailsResponse>>;
