using BuildingBlocks.Api.Problems;
using BuildingBlocks.Api.Scheduling;

namespace Host.Api.Composition;

/// <summary>What every module shares: errors as problem details, the pipeline, CORS, API docs, health checks.</summary>
internal static class PlatformRegistration
{
    public static IServiceCollection AddPlatform(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddApiProblems();
        services.AddMediatorPipeline();
        services.AddCorsForApps(configuration);
        services.AddApiDocumentation();
        services.AddHealthChecks();
        DatabaseStartup.AddWhenEnabled(services, configuration);
        services.Configure<OutboxTickOptions>(configuration.GetSection(OutboxTickOptions.SectionName));
        return services;
    }
}
