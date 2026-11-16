using BuildingBlocks.Api.Problems;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Catalog.Application.Features.Storefront.Queries.GetSitemap;

namespace Catalog.Api.Endpoints.Storefront;

internal static class GetSitemapEndpoint
{
    public static void MapGetSitemap(this RouteGroupBuilder publicApi) =>
        publicApi.MapGet("/sitemap", async (ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new GetSitemapQuery(), ct)).ToHttpResult(http))
            .WithName("getCatalogSitemap")
            .WithSummary("Every public catalog address, for sitemap.xml.")
            .Produces<SitemapResponse>();
}
