using Catalog.Contracts;
using Microsoft.Extensions.Time.Testing;
using Orders.Application.Features.Orders.Commands.PlaceOrder;
using Orders.Contracts;
using Orders.Domain.Orders;
using UnitTests.TestDoubles;

namespace UnitTests.Orders;

public sealed class PlaceOrderHandlerTests
{
    private static readonly ProductForSale Pen = new(Guid.NewGuid(), "Blue gel pen", 39m);
    private readonly TestOrdersDbContext _db = new();
    private readonly PlaceOrderHandler _handler;

    public PlaceOrderHandlerTests() =>
        _handler = new PlaceOrderHandler(_db, new FakeCatalog(Pen), new FakeTimeProvider(DateTimeOffset.UtcNow));

    private static PlaceOrderCommand Command(string key = "key-1", int quantity = 2) =>
        new(key, "Olena", "+380501112233", [new PlaceOrderLine(Pen.Id, quantity)]);

    [Fact]
    public async Task Places_TheOrder_AtCatalogPrices_AndQueuesOrderPlaced()
    {
        var result = await _handler.Handle(Command(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(78m, result.Value.Total);
        var message = Assert.IsType<OrderPlaced>(Assert.Single(_db.OutboxMessages));
        Assert.Equal(result.Value.OrderId, message.OrderId);
    }

    [Fact]
    public async Task A_Repeat_WithTheSameKeyAndData_ReturnsTheFirstOrder()
    {
        var first = await _handler.Handle(Command(), CancellationToken.None);
        var repeat = await _handler.Handle(Command(), CancellationToken.None);

        Assert.Equal(first.Value.OrderId, repeat.Value.OrderId);
        Assert.Single(_db.Orders);
        Assert.Single(_db.OutboxMessages);
    }

    [Fact]
    public async Task The_SameKey_WithOtherData_IsAConflict()
    {
        await _handler.Handle(Command(quantity: 2), CancellationToken.None);
        var other = await _handler.Handle(Command(quantity: 3), CancellationToken.None);

        Assert.Equal(OrderErrors.IdempotencyKeyReused, other.Error);
    }

    [Fact]
    public async Task A_ProductThatIsNotForSale_IsRefused()
    {
        var command = new PlaceOrderCommand("key-2", "Olena", "+380501112233", [new PlaceOrderLine(Guid.NewGuid(), 1)]);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(OrderErrors.ProductUnavailable, result.Error);
        Assert.Empty(_db.Orders);
    }
}
