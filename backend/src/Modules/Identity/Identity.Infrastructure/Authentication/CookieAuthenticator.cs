using BuildingBlocks.Domain.Results;
using Identity.Application.Features.Sessions;
using Identity.Application.Features.Sessions.Queries.GetCurrentUser;
using Identity.Domain.Users;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Authentication;

/// <summary>Cookie sign-in through ASP.NET Core Identity, with lockout after repeated wrong passwords.</summary>
internal sealed class CookieAuthenticator(
    SignInManager<StaffUser> signInManager,
    UserManager<StaffUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IAuthenticator
{
    public async Task<Result<CurrentUserResponse>> SignInAsync(
        string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return UserErrors.InvalidCredentials;

        var signIn = await signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: true);
        if (signIn.IsLockedOut)
            return UserErrors.LockedOut;
        if (!signIn.Succeeded)
            return UserErrors.InvalidCredentials;

        return await ToResponseAsync(user);
    }

    public Task SignOutAsync() => signInManager.SignOutAsync();

    public async Task<CurrentUserResponse?> FindCurrentUserAsync(CancellationToken cancellationToken)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return null;

        var user = await userManager.GetUserAsync(principal);
        return user is null ? null : await ToResponseAsync(user);
    }

    private async Task<CurrentUserResponse> ToResponseAsync(StaffUser user) =>
        new(user.Id, user.Email ?? string.Empty, user.DisplayName, [.. await userManager.GetRolesAsync(user)]);
}
