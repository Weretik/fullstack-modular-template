namespace Catalog.Application.Features.Products.Queries.GetProduct;

/// <summary>A product as staff edit it in the admin app, including drafts.</summary>
public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    decimal Price,
    Guid CategoryId,
    bool IsPublished,
    DateTimeOffset UpdatedAtUtc);
