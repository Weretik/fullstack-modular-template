using Orders.Application.Features.Orders.Commands.ChangeOrderStatus;
using Orders.Domain.Orders;
using UnitTests.TestDoubles;

namespace UnitTests.Orders;

public sealed class ChangeOrderStatusHandlerTests
{
    private readonly TestOrdersDbContext _db = new();

    private async Task<Order> StoredOrderAsync()
    {
        var line = OrderLine.Create(Guid.NewGuid(), "Pen", 10m, 1).Value;
        var order = Order.Place("Olena", "+380501112233", [line], Guid.NewGuid().ToString(), "hash", DateTimeOffset.UtcNow).Value;
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    [Fact]
    public async Task Moves_TheOrder_ToAnAllowedStatus()
    {
        var order = await StoredOrderAsync();

        var result = await new ChangeOrderStatusHandler(_db).Handle(
            new ChangeOrderStatusCommand(order.Id, OrderStatus.Confirmed), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Confirmed, _db.Orders.Single().Status);
    }

    [Fact]
    public async Task An_UnknownOrder_IsNotFound()
    {
        var result = await new ChangeOrderStatusHandler(_db).Handle(
            new ChangeOrderStatusCommand(Guid.NewGuid(), OrderStatus.Confirmed), CancellationToken.None);

        Assert.Equal(OrderErrors.NotFound, result.Error);
    }
}
