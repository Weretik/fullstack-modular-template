using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Catalog.Application.Features.Products.Commands.UpdateProduct;
using Catalog.Application.Features.Products.Queries.GetProduct;

namespace Catalog.Api.Endpoints.Products;

internal static class UpdateProductEndpoint
{
    public sealed record Request(string Name, string Slug, string? Description, decimal Price, Guid CategoryId, bool IsPublished);

    public static void MapUpdateProduct(this RouteGroupBuilder adminApi) =>
        adminApi.MapPut("/products/{id:guid}", async (
                Guid id, Request request, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new UpdateProductCommand(
                    id, request.Name, request.Slug, request.Description, request.Price, request.CategoryId, request.IsPublished), ct))
                    .ToHttpResult(http))
            .WithName("updateProduct")
            .WithSummary("Replace the data of a product, including whether it is published.")
            .Produces<ProductResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
