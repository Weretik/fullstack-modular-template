namespace Catalog.Application.Features.Products.Queries.SearchProducts;

public sealed record ProductListItem(Guid Id, string Name, string Slug, decimal Price, string CategoryName, bool IsPublished);
