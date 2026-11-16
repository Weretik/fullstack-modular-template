using BuildingBlocks.Api.Problems;
using Identity.Application.Features.Sessions.Queries.GetCurrentUser;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Endpoints.Sessions;

internal static class GetCurrentUserEndpoint
{
    public static void MapGetCurrentUser(this RouteGroupBuilder group) =>
        group.MapGet("/", async (ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new GetCurrentUserQuery(), ct)).ToHttpResult(http))
            .AllowAnonymous()
            .WithName("getCurrentUser")
            .WithSummary("The signed-in user, or 401 when nobody is signed in.")
            .Produces<CurrentUserResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
}
