using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Catalog.Application.Features.Storefront.Queries.GetPublishedProduct;

namespace Catalog.Api.Endpoints.Storefront;

internal static class GetPublishedProductEndpoint
{
    public static void MapGetPublishedProduct(this RouteGroupBuilder publicApi) =>
        publicApi.MapGet("/products/{slug}", async (string slug, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new GetPublishedProductQuery(slug), ct)).ToHttpResult(http))
            .WithName("getPublishedProduct")
            .WithSummary("The public page of a product.")
            .Produces<PublishedProductResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
