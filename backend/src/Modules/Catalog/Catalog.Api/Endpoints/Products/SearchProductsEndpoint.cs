using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using BuildingBlocks.Application.Persistence;
using Catalog.Application.Features.Products.Queries.SearchProducts;

namespace Catalog.Api.Endpoints.Products;

internal static class SearchProductsEndpoint
{
    public static void MapSearchProducts(this RouteGroupBuilder adminApi) =>
        adminApi.MapGet("/products", async (
                string? search, Guid? categoryId, int? page, int? pageSize,
                ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new SearchProductsQuery(search, categoryId, page ?? 1, pageSize ?? 20), ct)).ToHttpResult(http))
            .WithName("searchProducts")
            .WithSummary("Products for the admin list, drafts included.")
            .Produces<PagedList<ProductListItem>>()
            .ProducesValidationProblem();
}
