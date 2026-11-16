namespace Catalog.Application.Features.Storefront.Queries.GetSitemap;

/// <summary>One public address; categories have no change date.</summary>
public sealed record SitemapEntry(string Slug, DateTimeOffset? UpdatedAtUtc);
