namespace Orders.Contracts;

/// <summary>
/// Published through the Orders outbox after an order is saved. It is the public fact other modules (or a future
/// separate service) may react to; it carries data, never Orders domain types.
/// </summary>
public sealed record OrderPlaced(Guid OrderId, string CustomerName, decimal Total, DateTimeOffset PlacedAtUtc);
