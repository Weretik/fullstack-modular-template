using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Mediator;

namespace Catalog.Application.Features.Products.Queries.SearchProducts;

/// <summary>Products for the admin list, drafts included.</summary>
public sealed record SearchProductsQuery(string? Search, Guid? CategoryId, int Page = 1, int PageSize = 20)
    : IQuery<Result<PagedList<ProductListItem>>>;
