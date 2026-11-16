using BuildingBlocks.Domain.Results;
using Mediator;

namespace Catalog.Application.Features.Storefront.Queries.GetPublishedProduct;

public sealed record GetPublishedProductQuery(string Slug) : IQuery<Result<PublishedProductResponse>>;
