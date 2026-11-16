using BuildingBlocks.Infrastructure.Persistence;
using Identity.Domain.Users;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Seeding;

/// <summary>Applies the Identity migrations, creates the roles, and the first admin when one is configured.</summary>
internal sealed class IdentityDatabaseInitializer(
    IdentityStoreDbContext db,
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<StaffUser> userManager,
    IOptions<StaffSeedOptions> options) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await db.Database.MigrateAsync(cancellationToken);

        foreach (var role in Role.Staff)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var admin = options.Value;
        if (string.IsNullOrWhiteSpace(admin.Email) || await userManager.FindByEmailAsync(admin.Email) is not null)
            return;

        var user = new StaffUser { UserName = admin.Email, Email = admin.Email, DisplayName = admin.DisplayName, EmailConfirmed = true };
        var created = await userManager.CreateAsync(user, admin.Password);
        if (!created.Succeeded)
            throw new InvalidOperationException(string.Join("; ", created.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, Role.Admin);
    }
}
