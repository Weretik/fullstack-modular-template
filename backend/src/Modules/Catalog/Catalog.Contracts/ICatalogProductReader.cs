namespace Catalog.Contracts;

/// <summary>What another module may ask the catalog about products. It is the only way into Catalog data.</summary>
public interface ICatalogProductReader
{
    /// <summary>The current name and price of the given products; unknown and unpublished ones are left out.</summary>
    Task<IReadOnlyList<ProductForSale>> GetForSaleAsync(IReadOnlyCollection<Guid> productIds, CancellationToken cancellationToken);
}
