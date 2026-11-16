using Identity.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Api.Endpoints.Orders;
using Orders.Application;
using Orders.Infrastructure;

namespace Orders.Api;

/// <summary>The only entry point the host uses: register the module, then map its endpoints.</summary>
public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration) =>
        services.AddOrdersApplication().AddOrdersInfrastructure(configuration);

    public static IEndpointRouteBuilder MapOrdersModule(this IEndpointRouteBuilder endpoints)
    {
        var publicApi = endpoints.MapGroup("/api/orders").WithTags("Orders").AllowAnonymous();
        var adminApi = endpoints.MapGroup("/api/admin/orders").WithTags("Orders admin")
            .RequireAuthorization(IdentityPolicies.Staff)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        OrderEndpoints.MapOrderEndpoints(publicApi, adminApi);
        return endpoints;
    }
}
