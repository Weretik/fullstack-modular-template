using BuildingBlocks.Domain.Results;

namespace Identity.Domain.Users;

public static class UserErrors
{
    /// <summary>One message for a wrong e-mail and a wrong password, so the answer does not reveal which accounts exist.</summary>
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("identity.invalid_credentials", "Wrong e-mail or password.");

    public static readonly Error LockedOut =
        Error.Forbidden("identity.locked_out", "Too many sign-in attempts. Try again later.");

    public static readonly Error NotSignedIn =
        Error.Unauthorized("identity.not_signed_in", "Sign in first.");
}
