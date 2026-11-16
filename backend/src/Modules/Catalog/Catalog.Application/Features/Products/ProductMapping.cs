using System.Linq.Expressions;
using Catalog.Application.Features.Products.Queries.GetProduct;
using Catalog.Domain.Products;

namespace Catalog.Application.Features.Products;

/// <summary>One projection of a product for the admin app, used by every product use case that returns it.</summary>
internal static class ProductMapping
{
    public static readonly Expression<Func<Product, ProductResponse>> ToResponse = p =>
        new ProductResponse(p.Id, p.Name, p.Slug, p.Description, p.Price, p.CategoryId, p.IsPublished, p.UpdatedAtUtc);

    private static readonly Func<Product, ProductResponse> Compiled = ToResponse.Compile();

    public static ProductResponse ToResponseOf(Product product) => Compiled(product);
}
