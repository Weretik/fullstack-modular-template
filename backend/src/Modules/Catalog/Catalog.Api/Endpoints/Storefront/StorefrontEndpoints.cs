using Microsoft.AspNetCore.Routing;

namespace Catalog.Api.Endpoints.Storefront;

internal static class StorefrontEndpoints
{
    public static void MapStorefrontEndpoints(RouteGroupBuilder publicApi)
    {
        publicApi.MapListPublishedProducts();
        publicApi.MapGetPublishedProduct();
        publicApi.MapGetSitemap();
    }
}
