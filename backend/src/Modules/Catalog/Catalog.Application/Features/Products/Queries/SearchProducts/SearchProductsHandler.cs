using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Queries.SearchProducts;

public sealed class SearchProductsHandler(ICatalogDbContext db)
    : IQueryHandler<SearchProductsQuery, Result<PagedList<ProductListItem>>>
{
    public async ValueTask<Result<PagedList<ProductListItem>>> Handle(
        SearchProductsQuery query, CancellationToken cancellationToken)
    {
        var products = db.Products.AsNoTracking();
        foreach (var pattern in SearchPattern.ContainsEachWord(query.Search))
            products = products.Where(p => EF.Functions.Like(p.SearchText, pattern, SearchPattern.EscapeCharacter));
        if (query.CategoryId is { } categoryId)
            products = products.Where(p => p.CategoryId == categoryId);

        var page = await products
            .Join(db.Categories, p => p.CategoryId, c => c.Id, (p, c) => new { p, c })
            .OrderBy(x => x.p.Name)
            .ThenBy(x => x.p.Id)
            .Select(x => new ProductListItem(x.p.Id, x.p.Name, x.p.Slug, x.p.Price, x.c.Name, x.p.IsPublished))
            .ToPagedListAsync(query.Page, query.PageSize, cancellationToken);
        return page;
    }
}
