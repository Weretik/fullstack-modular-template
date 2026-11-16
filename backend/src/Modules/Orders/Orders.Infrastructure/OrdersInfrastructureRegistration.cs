using BuildingBlocks.Infrastructure.Outbox;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application;
using Orders.Application.Features.Notifications;
using Orders.Contracts;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure;

public static class OrdersInfrastructureRegistration
{
    public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<OrdersDbContext>(configuration, OrdersDbContext.Schema);
        services.AddScoped<IOrdersDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());
        services.AddScoped<IDatabaseInitializer, OrdersDatabaseInitializer>();

        services.AddModuleOutbox<OrdersDbContext>();
        services.AddOutboxMessage<OrderPlaced, OrderPlacedHandler>("orders.order-placed");
        services.AddScoped<IOrderNotifier, LoggingOrderNotifier>();
        return services;
    }
}
