using BuildingBlocks.Domain.Modeling;
using BuildingBlocks.Domain.Results;

namespace Orders.Domain.Orders;

/// <summary>
/// An order a customer placed on the site. It is stored under the client's idempotency key, so a repeated submit
/// finds the first order instead of creating a second one.
/// </summary>
public sealed class Order : Entity<Guid>, IAggregateRoot
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedChanges = new()
    {
        [OrderStatus.New] = [OrderStatus.Confirmed, OrderStatus.Cancelled],
        [OrderStatus.Confirmed] = [OrderStatus.Completed, OrderStatus.Cancelled],
        [OrderStatus.Completed] = [],
        [OrderStatus.Cancelled] = [],
    };

    public const int CustomerNameMaxLength = 100;

    private readonly List<OrderLine> _lines = [];

    private Order()
    {
    }

    public string CustomerName { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public OrderStatus Status { get; private set; }

    public IReadOnlyList<OrderLine> Lines => _lines;

    public decimal Total { get; private set; }

    public DateTimeOffset PlacedAtUtc { get; private set; }

    public string IdempotencyKey { get; private set; } = string.Empty;

    /// <summary>A hash of the request data: a repeat with the same key but other data is refused.</summary>
    public string RequestHash { get; private set; } = string.Empty;

    /// <summary>Row version: two staff changing the same order at once cannot overwrite each other.</summary>
    public uint Version { get; private set; }

    public static Result<Order> Place(
        string customerName,
        string phone,
        IReadOnlyCollection<OrderLine> lines,
        string idempotencyKey,
        string requestHash,
        DateTimeOffset nowUtc)
    {
        if (lines.Count == 0)
            return OrderErrors.Empty;

        var order = new Order
        {
            Id = Guid.CreateVersion7(nowUtc),
            CustomerName = customerName.Trim(),
            Phone = phone.Trim(),
            Status = OrderStatus.New,
            Total = lines.Sum(line => line.LineTotal),
            PlacedAtUtc = nowUtc,
            IdempotencyKey = idempotencyKey,
            RequestHash = requestHash,
        };
        order._lines.AddRange(lines);
        return order;
    }

    public Result ChangeStatus(OrderStatus status)
    {
        if (status == Status)
            return Result.Success();

        if (!AllowedChanges[Status].Contains(status))
            return OrderErrors.StatusChangeNotAllowed;

        Status = status;
        return Result.Success();
    }
}
