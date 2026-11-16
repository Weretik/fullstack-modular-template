using BuildingBlocks.Domain.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Storefront.Queries.GetSitemap;

public sealed class GetSitemapHandler(ICatalogDbContext db) : IQueryHandler<GetSitemapQuery, Result<SitemapResponse>>
{
    public async ValueTask<Result<SitemapResponse>> Handle(GetSitemapQuery query, CancellationToken cancellationToken)
    {
        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Slug)
            .Select(c => new SitemapEntry(c.Slug, null))
            .ToListAsync(cancellationToken);
        var products = await db.Products
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderBy(p => p.Slug)
            .Select(p => new SitemapEntry(p.Slug, p.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
        return new SitemapResponse(categories, products);
    }
}
