using Catalog.Contracts;

namespace UnitTests.TestDoubles;

/// <summary>Stands in for the Catalog module: Orders only ever sees it through its contract.</summary>
internal sealed class FakeCatalog(params ProductForSale[] products) : ICatalogProductReader
{
    public Task<IReadOnlyList<ProductForSale>> GetForSaleAsync(
        IReadOnlyCollection<Guid> productIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<ProductForSale>>([.. products.Where(p => productIds.Contains(p.Id))]);
}
