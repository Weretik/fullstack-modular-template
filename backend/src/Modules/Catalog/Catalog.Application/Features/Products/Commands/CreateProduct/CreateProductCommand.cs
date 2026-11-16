using BuildingBlocks.Domain.Results;
using Catalog.Application.Features.Products.Queries.GetProduct;
using Mediator;

namespace Catalog.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, string Slug, string? Description, decimal Price, Guid CategoryId)
    : ICommand<Result<ProductResponse>>;
