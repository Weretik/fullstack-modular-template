using BuildingBlocks.Domain.Results;

namespace Orders.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error Empty = Error.Validation(
        "orders.empty", "Add at least one product.", "lines");

    public static readonly Error InvalidQuantity = Error.Validation(
        "orders.invalid_quantity", "The quantity must be from 1 to 1000.", "quantity");

    public static readonly Error ProductUnavailable = Error.Validation(
        "orders.product_unavailable", "One of the products is no longer for sale. Refresh the cart.", "lines");

    public static readonly Error NotFound = Error.NotFound(
        "orders.not_found", "Order not found.");

    public static readonly Error StatusChangeNotAllowed = Error.Conflict(
        "orders.status_change_not_allowed", "An order in this status cannot change that way.");

    public static readonly Error IdempotencyKeyReused = Error.Conflict(
        "orders.idempotency_key_reused", "This idempotency key was already used for another order.");

    public static readonly Error ChangedByOtherUser = Error.Conflict(
        "orders.changed_by_other_user", "Someone else has just changed this order. Reload and try again.");
}
