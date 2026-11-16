using System.Net.Http.Json;

namespace IntegrationTests.TestHost;

internal static class HttpClientExtensions
{
    public static async Task SignInAsAdminAsync(this HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/session/sign-in",
            new { email = ApiFactory.AdminEmail, password = ApiFactory.AdminPassword });
        response.EnsureSuccessStatusCode();
    }
}
