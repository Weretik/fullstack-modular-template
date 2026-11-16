namespace Catalog.Application.Features.Storefront.Queries.GetPublishedProduct;

/// <summary>The public product page.</summary>
public sealed record PublishedProductResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    decimal Price,
    string CategoryName,
    string CategorySlug,
    DateTimeOffset UpdatedAtUtc);
