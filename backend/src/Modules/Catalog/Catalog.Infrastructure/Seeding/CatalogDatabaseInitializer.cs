using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Domain;
using Catalog.Domain.Categories;
using Catalog.Domain.Products;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Seeding;

/// <summary>Applies the Catalog migrations and, on an empty database, adds a small sample catalog.</summary>
internal sealed class CatalogDatabaseInitializer(CatalogDbContext db, TimeProvider clock) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await db.Database.MigrateAsync(cancellationToken);
        if (await db.Categories.AnyAsync(cancellationToken))
            return;

        var now = clock.GetUtcNow();
        foreach (var (category, products) in SampleCatalog.Items)
        {
            var created = Category.Create(category.Name, Slug.Create(category.Slug).Value).Value;
            db.Categories.Add(created);
            foreach (var (name, slug, price) in products)
            {
                var product = Product.Create(name, Slug.Create(slug).Value, $"{name}. Sample product.", price, created.Id, now).Value;
                product.Update(product.Name, Slug.Create(slug).Value, product.Description, price, created.Id, isPublished: true, now);
                db.Products.Add(product);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
