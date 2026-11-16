using BuildingBlocks.Domain.Results;

namespace Catalog.Domain;

public static class CatalogErrors
{
    public static readonly Error InvalidSlug = Error.Validation(
        "catalog.invalid_slug", "A slug may contain only lowercase latin letters, digits and single hyphens.", "slug");

    public static readonly Error SlugTaken = Error.Conflict(
        "catalog.slug_taken", "This slug is already in use.");
}
