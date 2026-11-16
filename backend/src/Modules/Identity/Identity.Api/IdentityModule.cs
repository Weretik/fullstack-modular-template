using Identity.Api.Endpoints.Sessions;
using Identity.Application;
using Identity.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api;

/// <summary>The only entry point the host uses: register the module, then map its endpoints.</summary>
public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration) =>
        services.AddIdentityApplication().AddIdentityInfrastructure(configuration);

    public static IEndpointRouteBuilder MapIdentityModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapSessionEndpoints();
        return endpoints;
    }
}
