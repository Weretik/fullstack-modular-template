using BuildingBlocks.Domain.Results;
using Catalog.Application.Features.Products.Queries.GetProduct;
using Mediator;

namespace Catalog.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id, string Name, string Slug, string? Description, decimal Price, Guid CategoryId, bool IsPublished)
    : ICommand<Result<ProductResponse>>;
