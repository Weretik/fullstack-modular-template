using Catalog.Domain.Categories;
using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application;

/// <summary>
/// The Catalog tables as the use cases see them. Commands change aggregates and save once; queries read with
/// <c>AsNoTracking</c> projections. No repository layer on top: EF Core already is one.
/// </summary>
public interface ICatalogDbContext
{
    DbSet<Category> Categories { get; }

    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
