namespace Identity.Contracts;

/// <summary>
/// Authorization policies other modules put on their endpoints. A module never checks roles itself; it names one
/// of these policies, and the Identity module decides who passes.
/// </summary>
public static class IdentityPolicies
{
    /// <summary>Staff who manage the catalog and orders in the admin app.</summary>
    public const string Staff = "staff";
}
