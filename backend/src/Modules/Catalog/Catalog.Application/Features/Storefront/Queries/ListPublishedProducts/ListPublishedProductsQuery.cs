using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Mediator;

namespace Catalog.Application.Features.Storefront.Queries.ListPublishedProducts;

/// <summary>Published products for the public site, optionally of one category or matching a search.</summary>
public sealed record ListPublishedProductsQuery(string? CategorySlug, string? Search, int Page = 1, int PageSize = 24)
    : IQuery<Result<PagedList<ProductCard>>>;
