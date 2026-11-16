using BuildingBlocks.Domain.Results;
using Identity.Application.Features.Sessions;
using Identity.Domain.Users;
using Mediator;

namespace Identity.Application.Features.Sessions.Queries.GetCurrentUser;

public sealed class GetCurrentUserHandler(IAuthenticator authenticator)
    : IQueryHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    public async ValueTask<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken) =>
        await authenticator.FindCurrentUserAsync(cancellationToken) is { } user
            ? user
            : UserErrors.NotSignedIn;
}
