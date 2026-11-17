using BuildingBlocks.Api.Problems;
using BuildingBlocks.Api.RateLimiting;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Features.Orders.Commands.PlaceOrder;

namespace Orders.Api.Endpoints.Orders;

internal static class PlaceOrderEndpoint
{
    public const string IdempotencyHeader = "Idempotency-Key";

    public sealed record Request(string CustomerName, string Phone, IReadOnlyList<RequestLine> Lines);

    public sealed record RequestLine(Guid ProductId, int Quantity);

    public static void MapPlaceOrder(this RouteGroupBuilder publicApi) =>
        publicApi.MapPost("/", async (
                [FromHeader(Name = IdempotencyHeader)] string? idempotencyKey,
                Request request, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new PlaceOrderCommand(
                    idempotencyKey ?? string.Empty,
                    request.CustomerName,
                    request.Phone,
                    [.. (request.Lines ?? []).Select(l => new PlaceOrderLine(l.ProductId, l.Quantity))]), ct))
                    .ToHttpResult(http))
            .RequireRateLimiting(RateLimitPolicies.PublicWrite)
            .WithName("placeOrder")
            .WithSummary("Place an order. Send one Idempotency-Key per checkout and repeat it on retry.")
            .Produces<PlacedOrderResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);
}
