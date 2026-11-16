using BuildingBlocks.Domain.Results;

namespace Catalog.Domain.Categories;

public static class CategoryErrors
{
    public static readonly Error InvalidName = Error.Validation(
        "catalog.category.invalid_name", "A category name is required, up to 100 characters.", "name");

    public static readonly Error NotFound = Error.NotFound(
        "catalog.category.not_found", "Category not found.");
}
