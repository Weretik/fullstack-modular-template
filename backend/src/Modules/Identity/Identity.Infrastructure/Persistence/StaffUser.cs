using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Persistence;

/// <summary>A staff account stored by ASP.NET Core Identity.</summary>
public sealed class StaffUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
}
