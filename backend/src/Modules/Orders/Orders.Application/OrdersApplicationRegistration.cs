using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Orders.Application;

public static class OrdersApplicationRegistration
{
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services) =>
        services.AddValidatorsFromAssembly(typeof(OrdersApplicationRegistration).Assembly, includeInternalTypes: true);
}
