using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using BuildingBlocks.Application.Persistence;
using Orders.Application.Features.Orders.Queries.ListOrders;
using Orders.Domain.Orders;

namespace Orders.Api.Endpoints.Orders;

internal static class ListOrdersEndpoint
{
    public static void MapListOrders(this RouteGroupBuilder adminApi) =>
        adminApi.MapGet("/", async (
                OrderStatus? status, int? page, int? pageSize, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new ListOrdersQuery(status, page ?? 1, pageSize ?? 20), ct)).ToHttpResult(http))
            .WithName("listOrders")
            .WithSummary("Orders for the admin list, newest first.")
            .Produces<PagedList<OrderListItem>>()
            .ProducesValidationProblem();
}
