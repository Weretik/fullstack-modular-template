using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Storefront.Queries.ListPublishedProducts;

public sealed class ListPublishedProductsHandler(ICatalogDbContext db)
    : IQueryHandler<ListPublishedProductsQuery, Result<PagedList<ProductCard>>>
{
    public async ValueTask<Result<PagedList<ProductCard>>> Handle(
        ListPublishedProductsQuery query, CancellationToken cancellationToken)
    {
        var products = db.Products.AsNoTracking().Where(p => p.IsPublished);
        foreach (var pattern in SearchPattern.ContainsEachWord(query.Search))
            products = products.Where(p => EF.Functions.Like(p.SearchText, pattern, SearchPattern.EscapeCharacter));

        var rows = products.Join(db.Categories, p => p.CategoryId, c => c.Id, (p, c) => new { p, c });
        if (!string.IsNullOrEmpty(query.CategorySlug))
            rows = rows.Where(x => x.c.Slug == query.CategorySlug);

        var page = await rows
            .OrderBy(x => x.p.Name)
            .ThenBy(x => x.p.Id)
            .Select(x => new ProductCard(x.p.Name, x.p.Slug, x.p.Price, x.c.Name, x.c.Slug))
            .ToPagedListAsync(query.Page, query.PageSize, cancellationToken);
        return page;
    }
}
