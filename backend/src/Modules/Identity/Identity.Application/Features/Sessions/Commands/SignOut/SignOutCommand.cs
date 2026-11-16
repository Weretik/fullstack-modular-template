using BuildingBlocks.Domain.Results;
using Mediator;

namespace Identity.Application.Features.Sessions.Commands.SignOut;

public sealed record SignOutCommand : ICommand<Result>;
