using BuildingBlocks.Api.Problems;
using Identity.Application.Features.Sessions.Commands.SignOut;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Endpoints.Sessions;

internal static class SignOutEndpoint
{
    public static void MapSignOut(this RouteGroupBuilder group) =>
        group.MapPost("/sign-out", async (ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new SignOutCommand(), ct)).ToHttpResult(http))
            .AllowAnonymous()
            .WithName("signOut")
            .WithSummary("Sign out and clear the session cookie.")
            .Produces(StatusCodes.Status204NoContent);
}
