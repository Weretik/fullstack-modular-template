namespace Identity.Domain.Users;

/// <summary>The roles a staff account can have.</summary>
public static class Role
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";

    public static readonly IReadOnlyList<string> Staff = [Admin, Manager];
}
