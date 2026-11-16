using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class CatalogInfrastructureRegistration
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<CatalogDbContext>(configuration, CatalogDbContext.Schema);
        services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());
        services.AddScoped<IDatabaseInitializer, CatalogDatabaseInitializer>();
        return services;
    }
}
