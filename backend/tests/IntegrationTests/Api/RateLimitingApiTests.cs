using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IntegrationTests.TestHost;
using Microsoft.AspNetCore.Hosting;

namespace IntegrationTests.Api;

[Collection(ApiCollection.Name)]
public sealed class RateLimitingApiTests(ApiFactory api)
{
    [SkippableFact]
    public async Task Sign_In_OverTheLimit_Answers429_WithRetryAfter_AndAProblem()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var limited = api.WithWebHostBuilder(builder => builder
            .UseSetting("RateLimiting:SignIn:PermitLimit", "2")
            .UseSetting("RateLimiting:SignIn:WindowSeconds", "60"));
        using var client = limited.CreateClient(new() { BaseAddress = new Uri("https://localhost") });

        var answers = new List<HttpResponseMessage>();
        for (var i = 0; i < 3; i++)
            answers.Add(await client.PostAsJsonAsync("/api/session/sign-in",
                new { email = ApiFactory.AdminEmail, password = "wrong-password" }));

        Assert.All(answers.Take(2), r => Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode));
        var rejected = answers[2];
        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.True(rejected.Headers.RetryAfter is not null, "429 has no Retry-After header");
        var problem = await rejected.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("too_many_requests", problem.GetProperty("code").GetString());
    }

    [SkippableFact]
    public async Task Reading_TheCatalog_IsNotLimited()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var limited = api.WithWebHostBuilder(builder => builder
            .UseSetting("RateLimiting:PublicWrite:PermitLimit", "1")
            .UseSetting("RateLimiting:SignIn:PermitLimit", "1"));
        using var client = limited.CreateClient(new() { BaseAddress = new Uri("https://localhost") });

        var answers = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => client.GetAsync("/api/catalog/products")));

        Assert.All(answers, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }
}
