using Catalog.Application.Features.Products;
using Catalog.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class CatalogApplicationRegistration
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CatalogApplicationRegistration).Assembly, includeInternalTypes: true);
        services.AddScoped<ICatalogProductReader, CatalogProductReader>();
        return services;
    }
}
