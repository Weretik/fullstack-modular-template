using BuildingBlocks.Domain.Results;
using Mediator;

namespace Identity.Application.Features.Sessions.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<Result<CurrentUserResponse>>;
