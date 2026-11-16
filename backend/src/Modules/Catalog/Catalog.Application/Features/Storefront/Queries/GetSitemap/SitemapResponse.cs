namespace Catalog.Application.Features.Storefront.Queries.GetSitemap;

/// <summary>Every public catalog address, for the site's sitemap.xml.</summary>
public sealed record SitemapResponse(IReadOnlyList<SitemapEntry> Categories, IReadOnlyList<SitemapEntry> Products);
