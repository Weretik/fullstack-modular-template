using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Catalog.Application.Features.Categories.Queries.ListCategories;

namespace Catalog.Api.Endpoints.Categories;

internal static class ListCategoriesEndpoint
{
    public static void MapListCategories(this RouteGroupBuilder publicApi) =>
        publicApi.MapGet("/categories", async (ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new ListCategoriesQuery(), ct)).ToHttpResult(http))
            .WithName("listCategories")
            .WithSummary("All categories, by name.")
            .Produces<IReadOnlyList<CategoryResponse>>();
}
