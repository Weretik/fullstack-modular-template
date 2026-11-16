using Microsoft.AspNetCore.Routing;

namespace Catalog.Api.Endpoints.Products;

internal static class ProductEndpoints
{
    public static void MapProductEndpoints(RouteGroupBuilder adminApi)
    {
        adminApi.MapSearchProducts();
        adminApi.MapGetProduct();
        adminApi.MapCreateProduct();
        adminApi.MapUpdateProduct();
    }
}
