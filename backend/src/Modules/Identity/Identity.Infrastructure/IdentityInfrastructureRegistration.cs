using BuildingBlocks.Infrastructure.Persistence;
using Identity.Application.Features.Sessions;
using Identity.Contracts;
using Identity.Domain.Users;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Seeding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class IdentityInfrastructureRegistration
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<IdentityStoreDbContext>(configuration, IdentityStoreDbContext.Schema);
        services.AddHttpContextAccessor();

        services
            .AddIdentityCore<StaffUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityStoreDbContext>()
            .AddSignInManager();

        services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.Cookie.Name = "session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                // Strict keeps the cookie off cross-site requests, which is the CSRF protection of the API.
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                // An API answers 401/403; it never redirects to a login page.
                options.Events.OnRedirectToLogin = context => Status(context.Response, StatusCodes.Status401Unauthorized);
                options.Events.OnRedirectToAccessDenied = context => Status(context.Response, StatusCodes.Status403Forbidden);
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(IdentityPolicies.Staff, policy => policy.RequireRole(Role.Staff));

        services.Configure<StaffSeedOptions>(configuration.GetSection(StaffSeedOptions.SectionName));
        services.AddScoped<IAuthenticator, CookieAuthenticator>();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
        services.AddScoped<IDatabaseInitializer, IdentityDatabaseInitializer>();
        return services;
    }

    private static Task Status(HttpResponse response, int status)
    {
        response.StatusCode = status;
        return Task.CompletedTask;
    }
}
