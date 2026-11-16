using Catalog.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products;

/// <summary>Answers other modules through <see cref="ICatalogProductReader"/>; only published products are for sale.</summary>
internal sealed class CatalogProductReader(ICatalogDbContext db) : ICatalogProductReader
{
    public async Task<IReadOnlyList<ProductForSale>> GetForSaleAsync(
        IReadOnlyCollection<Guid> productIds, CancellationToken cancellationToken) =>
        await db.Products
            .AsNoTracking()
            .Where(p => p.IsPublished && productIds.Contains(p.Id))
            .Select(p => new ProductForSale(p.Id, p.Name, p.Price))
            .ToListAsync(cancellationToken);
}
