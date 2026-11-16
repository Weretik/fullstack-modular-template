using Microsoft.AspNetCore.Routing;

namespace Catalog.Api.Endpoints.Categories;

internal static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(RouteGroupBuilder publicApi, RouteGroupBuilder adminApi)
    {
        publicApi.MapListCategories();
        adminApi.MapCreateCategory();
    }
}
