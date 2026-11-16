using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using BuildingBlocks.Application.Persistence;
using Catalog.Application.Features.Storefront.Queries.ListPublishedProducts;

namespace Catalog.Api.Endpoints.Storefront;

internal static class ListPublishedProductsEndpoint
{
    public static void MapListPublishedProducts(this RouteGroupBuilder publicApi) =>
        publicApi.MapGet("/products", async (
                string? category, string? search, int? page, int? pageSize,
                ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new ListPublishedProductsQuery(category, search, page ?? 1, pageSize ?? 24), ct)).ToHttpResult(http))
            .WithName("listPublishedProducts")
            .WithSummary("Published products of the public catalog.")
            .Produces<PagedList<ProductCard>>()
            .ProducesValidationProblem();
}
