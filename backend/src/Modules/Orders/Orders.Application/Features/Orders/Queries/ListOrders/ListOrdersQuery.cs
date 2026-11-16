using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Mediator;
using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Queries.ListOrders;

/// <summary>Orders for the admin list, newest first.</summary>
public sealed record ListOrdersQuery(OrderStatus? Status, int Page = 1, int PageSize = 20)
    : IQuery<Result<PagedList<OrderListItem>>>;
