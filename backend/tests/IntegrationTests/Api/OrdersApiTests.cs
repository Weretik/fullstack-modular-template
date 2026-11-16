using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IntegrationTests.TestHost;

namespace IntegrationTests.Api;

[Collection(ApiCollection.Name)]
public sealed class OrdersApiTests(ApiFactory api)
{
    private static async Task<Guid> PublishedProductIdAsync(HttpClient client)
    {
        await client.SignInAsAdminAsync();
        var page = await client.GetFromJsonAsync<JsonElement>("/api/admin/catalog/products?search=paper%20clips");
        return page.GetProperty("items")[0].GetProperty("id").GetGuid();
    }

    private static HttpRequestMessage PlaceOrder(string? key, Guid productId, int quantity)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/orders")
        {
            Content = JsonContent.Create(new
            {
                customerName = "Olena",
                phone = "+380501112233",
                lines = new[] { new { productId, quantity } },
            }),
        };
        if (key is not null)
            request.Headers.Add("Idempotency-Key", key);
        return request;
    }

    [SkippableFact]
    public async Task A_RepeatedCheckout_ReturnsTheFirstOrder_AndOtherDataUnderTheSameKeyIsAConflict()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();
        var productId = await PublishedProductIdAsync(client);
        var key = Guid.NewGuid().ToString();

        var first = await client.SendAsync(PlaceOrder(key, productId, 2));
        var repeat = await client.SendAsync(PlaceOrder(key, productId, 2));
        var other = await client.SendAsync(PlaceOrder(key, productId, 5));

        var firstBody = await first.Content.ReadFromJsonAsync<JsonElement>();
        var repeatBody = await repeat.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(firstBody.GetProperty("orderId").GetGuid(), repeatBody.GetProperty("orderId").GetGuid());
        Assert.Equal(58m, firstBody.GetProperty("total").GetDecimal());
        Assert.Equal(HttpStatusCode.Conflict, other.StatusCode);
    }

    [SkippableFact]
    public async Task Racing_Copies_OfOneCheckout_StoreOneOrder()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();
        var productId = await PublishedProductIdAsync(client);
        var key = Guid.NewGuid().ToString();

        var responses = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => client.SendAsync(PlaceOrder(key, productId, 1))));
        var ids = await Task.WhenAll(responses.Select(async r =>
            (await r.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("orderId").GetGuid()));

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
        Assert.Single(ids.Distinct());
    }

    [SkippableFact]
    public async Task A_Checkout_WithoutAnIdempotencyKey_IsRefused()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();
        var productId = await PublishedProductIdAsync(client);

        var response = await client.SendAsync(PlaceOrder(null, productId, 1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [SkippableFact]
    public async Task The_OutboxTick_SendsOrderPlaced_AndNeedsTheSchedulerKey()
    {
        Skip.If(api.Unavailable is not null, api.Unavailable);
        using var client = api.CreateHttpsClient();
        var productId = await PublishedProductIdAsync(client);
        await client.SendAsync(PlaceOrder(Guid.NewGuid().ToString(), productId, 1));

        var withoutKey = await client.PostAsync("/internal/outbox/tick", null);
        var tick = new HttpRequestMessage(HttpMethod.Post, "/internal/outbox/tick");
        tick.Headers.Add("X-Scheduler-Key", ApiFactory.TickKey);
        var report = await (await client.SendAsync(tick)).Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.Unauthorized, withoutKey.StatusCode);
        Assert.True(report.GetProperty("sent").GetInt32() >= 1);
    }
}
