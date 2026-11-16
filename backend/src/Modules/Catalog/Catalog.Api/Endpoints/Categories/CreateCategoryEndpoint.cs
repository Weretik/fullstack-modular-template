using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Catalog.Application.Features.Categories.Commands.CreateCategory;
using Catalog.Application.Features.Categories.Queries.ListCategories;

namespace Catalog.Api.Endpoints.Categories;

internal static class CreateCategoryEndpoint
{
    public sealed record Request(string Name, string Slug);

    public static void MapCreateCategory(this RouteGroupBuilder adminApi) =>
        adminApi.MapPost("/categories", async (Request request, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new CreateCategoryCommand(request.Name, request.Slug), ct))
                    .ToCreatedHttpResult(http, c => $"/api/catalog/categories/{c.Slug}"))
            .WithName("createCategory")
            .WithSummary("Create a category.")
            .Produces<CategoryResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
}
