using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Mediator;

namespace Orders.Application.Features.Orders.Queries.ListOrders;

public sealed class ListOrdersHandler(IOrdersDbContext db) : IQueryHandler<ListOrdersQuery, Result<PagedList<OrderListItem>>>
{
    public async ValueTask<Result<PagedList<OrderListItem>>> Handle(ListOrdersQuery query, CancellationToken cancellationToken)
    {
        var orders = db.Orders.AsQueryable();
        if (query.Status is { } status)
            orders = orders.Where(o => o.Status == status);

        var page = await orders
            .OrderByDescending(o => o.PlacedAtUtc)
            .ThenByDescending(o => o.Id)
            .Select(o => new OrderListItem(o.Id, o.CustomerName, o.Phone, o.Status, o.Total, o.PlacedAtUtc))
            .ToPagedListAsync(query.Page, query.PageSize, cancellationToken);
        return page;
    }
}
