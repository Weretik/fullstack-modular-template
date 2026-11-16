using BuildingBlocks.Domain.Results;

namespace Orders.Domain.Orders;

/// <summary>
/// One product of an order with the name and price it had when the order was placed, so later catalog changes do
/// not rewrite past orders.
/// </summary>
public sealed record OrderLine
{
    public const int MaxQuantity = 1_000;

    private OrderLine(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid ProductId { get; private init; }

    public string ProductName { get; private init; }

    public decimal UnitPrice { get; private init; }

    public int Quantity { get; private init; }

    public decimal LineTotal => UnitPrice * Quantity;

    public static Result<OrderLine> Create(Guid productId, string productName, decimal unitPrice, int quantity) =>
        quantity is < 1 or > MaxQuantity
            ? OrderErrors.InvalidQuantity
            : new OrderLine(productId, productName, unitPrice, quantity);
}
