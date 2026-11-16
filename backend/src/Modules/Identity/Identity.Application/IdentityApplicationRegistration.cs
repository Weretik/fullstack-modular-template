using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application;

public static class IdentityApplicationRegistration
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services) =>
        services.AddValidatorsFromAssembly(typeof(IdentityApplicationRegistration).Assembly, includeInternalTypes: true);
}
