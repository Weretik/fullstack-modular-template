using BuildingBlocks.Domain.Results;
using Identity.Application.Features.Sessions;
using Mediator;

namespace Identity.Application.Features.Sessions.Commands.SignOut;

/// <summary>Signing out twice is fine: the second call has nothing to clear and still succeeds.</summary>
public sealed class SignOutHandler(IAuthenticator authenticator) : ICommandHandler<SignOutCommand, Result>
{
    public async ValueTask<Result> Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        await authenticator.SignOutAsync();
        return Result.Success();
    }
}
