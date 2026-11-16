using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Catalog.Application.Features.Products.Commands.CreateProduct;
using Catalog.Application.Features.Products.Queries.GetProduct;

namespace Catalog.Api.Endpoints.Products;

internal static class CreateProductEndpoint
{
    public sealed record Request(string Name, string Slug, string? Description, decimal Price, Guid CategoryId);

    public static void MapCreateProduct(this RouteGroupBuilder adminApi) =>
        adminApi.MapPost("/products", async (Request request, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(
                    new CreateProductCommand(request.Name, request.Slug, request.Description, request.Price, request.CategoryId), ct))
                    .ToCreatedHttpResult(http, p => $"/api/admin/catalog/products/{p.Id}"))
            .WithName("createProduct")
            .WithSummary("Create a draft product.")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
}
