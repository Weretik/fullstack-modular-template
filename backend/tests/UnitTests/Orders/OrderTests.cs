using Orders.Domain.Orders;

namespace UnitTests.Orders;

public sealed class OrderTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 10, 9, 0, 0, TimeSpan.Zero);

    private static OrderLine Line(decimal price, int quantity) =>
        OrderLine.Create(Guid.NewGuid(), "Pen", price, quantity).Value;

    [Fact]
    public void Place_SumsTheLines_AndStartsAsNew()
    {
        var order = Order.Place("Olena", "+380501112233", [Line(10m, 2), Line(2.5m, 4)], "key-1", "hash", Now);

        Assert.True(order.IsSuccess);
        Assert.Equal(30m, order.Value.Total);
        Assert.Equal(OrderStatus.New, order.Value.Status);
    }

    [Fact]
    public void Place_RejectsAnOrderWithoutLines()
    {
        var order = Order.Place("Olena", "+380501112233", [], "key-1", "hash", Now);

        Assert.Equal(OrderErrors.Empty, order.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public void LineCreate_RejectsAQuantityOutOfRange(int quantity)
    {
        var line = OrderLine.Create(Guid.NewGuid(), "Pen", 10m, quantity);

        Assert.Equal(OrderErrors.InvalidQuantity, line.Error);
    }

    [Fact]
    public void Confirm_ThenCancel_IsNotAllowed()
    {
        var order = Order.Place("Olena", "+380501112233", [Line(10m, 1)], "key-1", "hash", Now).Value;

        Assert.True(order.ChangeStatus(OrderStatus.Confirmed).IsSuccess);
        Assert.True(order.ChangeStatus(OrderStatus.Completed).IsSuccess);
        Assert.Equal(OrderErrors.StatusChangeNotAllowed, order.ChangeStatus(OrderStatus.Cancelled).Error);
    }

    [Fact]
    public void ChangeStatus_ToTheSameStatus_SucceedsWithoutChange()
    {
        var order = Order.Place("Olena", "+380501112233", [Line(10m, 1)], "key-1", "hash", Now).Value;

        Assert.True(order.ChangeStatus(OrderStatus.New).IsSuccess);
        Assert.Equal(OrderStatus.New, order.Status);
    }
}
