namespace Identity.Infrastructure.Seeding;

/// <summary>The first admin account for local development (section <c>Identity:SeedAdmin</c>).</summary>
public sealed class StaffSeedOptions
{
    public const string SectionName = "Identity:SeedAdmin";

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string DisplayName { get; set; } = "Admin";
}
