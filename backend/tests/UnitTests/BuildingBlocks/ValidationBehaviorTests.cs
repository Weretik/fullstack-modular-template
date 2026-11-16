using BuildingBlocks.Application.Pipeline;
using BuildingBlocks.Domain.Results;
using Identity.Application.Features.Sessions.Commands.SignIn;
using Identity.Application.Features.Sessions.Queries.GetCurrentUser;

namespace UnitTests.BuildingBlocks;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Invalid_Input_ReturnsAValidationError_PerField_WithoutCallingTheHandler()
    {
        var behavior = new ValidationBehavior<SignInCommand, Result<CurrentUserResponse>>([new SignInValidator()]);
        var called = false;

        var result = await behavior.Handle(new SignInCommand("not-an-email", ""), (_, _) =>
        {
            called = true;
            return ValueTask.FromResult(Result<CurrentUserResponse>.Failure(Error.Conflict("x", "x")));
        }, CancellationToken.None);

        Assert.False(called);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        Assert.Contains("Email", result.Error.Fields!.Keys);
        Assert.Contains("Password", result.Error.Fields.Keys);
    }
}
