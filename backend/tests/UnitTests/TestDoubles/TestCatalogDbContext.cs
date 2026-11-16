using Catalog.Application;
using Catalog.Domain.Categories;
using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.TestDoubles;

/// <summary>An in-memory Catalog database: fast, and enough for handler rules that do not depend on SQL.</summary>
internal sealed class TestCatalogDbContext() : DbContext(
    new DbContextOptionsBuilder<TestCatalogDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options), ICatalogDbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();
}
