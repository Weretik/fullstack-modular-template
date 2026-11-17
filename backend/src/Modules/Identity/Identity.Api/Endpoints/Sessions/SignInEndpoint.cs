using BuildingBlocks.Api.Problems;
using BuildingBlocks.Api.RateLimiting;
using Identity.Application.Features.Sessions.Commands.SignIn;
using Identity.Application.Features.Sessions.Queries.GetCurrentUser;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Endpoints.Sessions;

internal static class SignInEndpoint
{
    public sealed record Request(string Email, string Password);

    public static void MapSignIn(this RouteGroupBuilder group) =>
        group.MapPost("/sign-in", async (Request request, ISender sender, HttpContext http, CancellationToken ct) =>
                (await sender.Send(new SignInCommand(request.Email, request.Password), ct)).ToHttpResult(http))
            .AllowAnonymous()
            .RequireRateLimiting(RateLimitPolicies.SignIn)
            .WithName("signIn")
            .WithSummary("Sign in with e-mail and password; sets the session cookie.")
            .Produces<CurrentUserResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);
}
