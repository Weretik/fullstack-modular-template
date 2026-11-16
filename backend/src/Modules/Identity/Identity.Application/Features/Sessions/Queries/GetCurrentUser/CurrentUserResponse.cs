namespace Identity.Application.Features.Sessions.Queries.GetCurrentUser;

public sealed record CurrentUserResponse(Guid Id, string Email, string DisplayName, IReadOnlyList<string> Roles);
