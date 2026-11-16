using BuildingBlocks.Domain.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Queries.ListCategories;

public sealed class ListCategoriesHandler(ICatalogDbContext db)
    : IQueryHandler<ListCategoriesQuery, Result<IReadOnlyList<CategoryResponse>>>
{
    public async ValueTask<Result<IReadOnlyList<CategoryResponse>>> Handle(
        ListCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Slug))
            .ToListAsync(cancellationToken);
        return categories;
    }
}
