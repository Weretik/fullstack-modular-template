using BuildingBlocks.Domain.Results;
using Catalog.Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Storefront.Queries.GetPublishedProduct;

/// <summary>A draft answers 404 exactly like a missing product, so the site never leaks unpublished data.</summary>
public sealed class GetPublishedProductHandler(ICatalogDbContext db)
    : IQueryHandler<GetPublishedProductQuery, Result<PublishedProductResponse>>
{
    public async ValueTask<Result<PublishedProductResponse>> Handle(
        GetPublishedProductQuery query, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .AsNoTracking()
            .Where(p => p.IsPublished && p.Slug == query.Slug)
            .Join(db.Categories, p => p.CategoryId, c => c.Id, (p, c) => new PublishedProductResponse(
                p.Id, p.Name, p.Slug, p.Description, p.Price, c.Name, c.Slug, p.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
        return product is null ? ProductErrors.NotFound : product;
    }
}
