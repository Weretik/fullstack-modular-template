using BuildingBlocks.Domain.Results;
using Mediator;

namespace Catalog.Application.Features.Storefront.Queries.GetSitemap;

public sealed record GetSitemapQuery : IQuery<Result<SitemapResponse>>;
