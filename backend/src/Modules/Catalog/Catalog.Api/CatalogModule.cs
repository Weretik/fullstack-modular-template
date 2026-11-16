using Catalog.Api.Endpoints.Categories;
using Catalog.Api.Endpoints.Products;
using Catalog.Api.Endpoints.Storefront;
using Catalog.Application;
using Catalog.Infrastructure;
using Identity.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Api;

/// <summary>The only entry point the host uses: register the module, then map its endpoints.</summary>
public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration) =>
        services.AddCatalogApplication().AddCatalogInfrastructure(configuration);

    public static IEndpointRouteBuilder MapCatalogModule(this IEndpointRouteBuilder endpoints)
    {
        var publicApi = endpoints.MapGroup("/api/catalog").WithTags("Catalog").AllowAnonymous();
        var adminApi = endpoints.MapGroup("/api/admin/catalog").WithTags("Catalog admin")
            .RequireAuthorization(IdentityPolicies.Staff)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        CategoryEndpoints.MapCategoryEndpoints(publicApi, adminApi);
        ProductEndpoints.MapProductEndpoints(adminApi);
        StorefrontEndpoints.MapStorefrontEndpoints(publicApi);
        return endpoints;
    }
}
