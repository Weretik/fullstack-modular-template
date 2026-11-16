using BuildingBlocks.Domain.Results;
using Mediator;

namespace Catalog.Application.Features.Products.Queries.GetProduct;

public sealed record GetProductQuery(Guid Id) : IQuery<Result<ProductResponse>>;
