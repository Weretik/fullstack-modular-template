using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Orders.Application.Features.Orders.Queries.GetOrder;

namespace Orders.Api.Endpoints.Orders;

internal static class GetOrderEndpoint
{
    public static void MapGetOrder(this RouteGroupBuilder adminApi) =>
        adminApi.MapGet("/{id:guid}", async (Guid id, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new GetOrderQuery(id), ct)).ToHttpResult(http))
            .WithName("getOrder")
            .WithSummary("One order with its lines.")
            .Produces<OrderDetailsResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
