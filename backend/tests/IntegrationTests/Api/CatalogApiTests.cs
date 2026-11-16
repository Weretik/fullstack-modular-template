using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IntegrationTests.TestHost;

namespace IntegrationTests.Api;

[Collection(ApiCollection.Name)]
public sealed class CatalogApiTests(ApiFactory api)
{
    [SkippableFact]
    public async Task The_PublicCatalog_ShowsPublishedSampleProducts()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();

        var page = await client.GetFromJsonAsync<JsonElement>("/api/catalog/products?category=pens");
        var product = await client.GetAsync("/api/catalog/products/fountain-pen");

        Assert.Equal(3, page.GetProperty("totalCount").GetInt32());
        Assert.Equal(HttpStatusCode.OK, product.StatusCode);
    }

    [SkippableFact]
    public async Task Search_MatchesEveryWord()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();

        var page = await client.GetFromJsonAsync<JsonElement>("/api/catalog/products?search=GEL%20blue");

        Assert.Equal("blue-gel-pen", page.GetProperty("items")[0].GetProperty("slug").GetString());
    }

    [SkippableFact]
    public async Task Admin_Endpoints_NeedASignedInStaffMember()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();

        var response = await client.GetAsync("/api/admin/catalog/products");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [SkippableFact]
    public async Task Staff_CreateADraft_ThatTheSiteDoesNotShow_AndASecondCopyIsAConflict()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();
        await client.SignInAsAdminAsync();
        var categories = await client.GetFromJsonAsync<JsonElement>("/api/catalog/categories");
        var categoryId = categories[0].GetProperty("id").GetGuid();
        var product = new { name = "Draft marker", slug = "draft-marker", description = (string?)null, price = 55.5m, categoryId };

        var created = await client.PostAsJsonAsync("/api/admin/catalog/products", product);
        var repeat = await client.PostAsJsonAsync("/api/admin/catalog/products", product);
        var publicPage = await client.GetAsync("/api/catalog/products/draft-marker");

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, repeat.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, publicPage.StatusCode);
    }

    [SkippableFact]
    public async Task Invalid_Input_IsAProblem_WithAMessagePerField()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();
        await client.SignInAsAdminAsync();

        var response = await client.PostAsJsonAsync("/api/admin/catalog/products",
            new { name = "", slug = "Bad Slug", price = -1m, categoryId = Guid.Empty });
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("validation", problem.GetProperty("code").GetString());
        Assert.True(problem.GetProperty("errors").TryGetProperty("Name", out _));
    }
}
