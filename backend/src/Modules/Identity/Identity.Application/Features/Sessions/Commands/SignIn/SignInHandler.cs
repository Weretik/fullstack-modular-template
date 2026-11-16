using BuildingBlocks.Domain.Results;
using Identity.Application.Features.Sessions;
using Identity.Application.Features.Sessions.Queries.GetCurrentUser;
using Mediator;

namespace Identity.Application.Features.Sessions.Commands.SignIn;

public sealed class SignInHandler(IAuthenticator authenticator)
    : ICommandHandler<SignInCommand, Result<CurrentUserResponse>>
{
    public async ValueTask<Result<CurrentUserResponse>> Handle(SignInCommand command, CancellationToken cancellationToken) =>
        await authenticator.SignInAsync(command.Email.Trim(), command.Password, cancellationToken);
}
