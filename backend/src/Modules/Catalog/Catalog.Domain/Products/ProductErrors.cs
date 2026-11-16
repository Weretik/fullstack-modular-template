using BuildingBlocks.Domain.Results;

namespace Catalog.Domain.Products;

public static class ProductErrors
{
    public static readonly Error InvalidName = Error.Validation(
        "catalog.product.invalid_name", "A product name is required, up to 200 characters.", "name");

    public static readonly Error InvalidPrice = Error.Validation(
        "catalog.product.invalid_price", "The price must be above zero with at most two decimals.", "price");

    public static readonly Error DescriptionTooLong = Error.Validation(
        "catalog.product.description_too_long", "The description must be up to 4000 characters.", "description");

    public static readonly Error UnknownCategory = Error.Validation(
        "catalog.product.unknown_category", "Choose an existing category.", "categoryId");

    public static readonly Error NotFound = Error.NotFound(
        "catalog.product.not_found", "Product not found.");
}
