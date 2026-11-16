using Microsoft.AspNetCore.Routing;

namespace Orders.Api.Endpoints.Orders;

internal static class OrderEndpoints
{
    public static void MapOrderEndpoints(RouteGroupBuilder publicApi, RouteGroupBuilder adminApi)
    {
        publicApi.MapPlaceOrder();
        adminApi.MapListOrders();
        adminApi.MapGetOrder();
        adminApi.MapChangeOrderStatus();
    }
}
