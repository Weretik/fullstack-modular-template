using BuildingBlocks.Application.Pipeline;

namespace Host.Api.Composition;

internal static class MediatorSetup
{
    /// <summary>
    /// One pipeline for every command and query: log, then validate, then the handler. The source generator finds
    /// the handlers of each module's Application assembly at compile time.
    /// </summary>
    public static IServiceCollection AddMediatorPipeline(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.Assemblies =
            [
                typeof(Identity.Application.IdentityApplicationRegistration).Assembly,
                typeof(Catalog.Application.CatalogApplicationRegistration).Assembly,
                typeof(Orders.Application.OrdersApplicationRegistration).Assembly,
            ];
            options.PipelineBehaviors = [typeof(LoggingBehavior<,>), typeof(ValidationBehavior<,>)];
        });
        return services;
    }
}
