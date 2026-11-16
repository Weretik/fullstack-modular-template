using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Catalog.Application.Features.Products.Queries.GetProduct;

namespace Catalog.Api.Endpoints.Products;

internal static class GetProductEndpoint
{
    public static void MapGetProduct(this RouteGroupBuilder adminApi) =>
        adminApi.MapGet("/products/{id:guid}", async (Guid id, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new GetProductQuery(id), ct)).ToHttpResult(http))
            .WithName("getProduct")
            .WithSummary("One product for editing.")
            .Produces<ProductResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
