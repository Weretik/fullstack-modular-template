using BuildingBlocks.Domain.Results;
using Identity.Application.Features.Sessions.Queries.GetCurrentUser;
using Mediator;

namespace Identity.Application.Features.Sessions.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password) : ICommand<Result<CurrentUserResponse>>;
