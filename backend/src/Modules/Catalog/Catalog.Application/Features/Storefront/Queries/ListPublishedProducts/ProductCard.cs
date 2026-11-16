namespace Catalog.Application.Features.Storefront.Queries.ListPublishedProducts;

/// <summary>A product tile of the public catalog.</summary>
public sealed record ProductCard(string Name, string Slug, decimal Price, string CategoryName, string CategorySlug);
