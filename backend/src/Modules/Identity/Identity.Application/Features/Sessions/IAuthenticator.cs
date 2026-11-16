using BuildingBlocks.Domain.Results;
using Identity.Application.Features.Sessions.Queries.GetCurrentUser;

namespace Identity.Application.Features.Sessions;

/// <summary>Signs a user in and out with a cookie; Infrastructure implements it with ASP.NET Core Identity.</summary>
public interface IAuthenticator
{
    Task<Result<CurrentUserResponse>> SignInAsync(string email, string password, CancellationToken cancellationToken);

    Task SignOutAsync();

    Task<CurrentUserResponse?> FindCurrentUserAsync(CancellationToken cancellationToken);
}
