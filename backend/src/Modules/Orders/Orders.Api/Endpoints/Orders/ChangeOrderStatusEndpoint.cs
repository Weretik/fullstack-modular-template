using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Orders.Application.Features.Orders.Commands.ChangeOrderStatus;
using Orders.Domain.Orders;

namespace Orders.Api.Endpoints.Orders;

internal static class ChangeOrderStatusEndpoint
{
    public sealed record Request(OrderStatus Status);

    public static void MapChangeOrderStatus(this RouteGroupBuilder adminApi) =>
        adminApi.MapPut("/{id:guid}/status", async (
                Guid id, Request request, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new ChangeOrderStatusCommand(id, request.Status), ct)).ToHttpResult(http))
            .WithName("changeOrderStatus")
            .WithSummary("Move an order to another status.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
