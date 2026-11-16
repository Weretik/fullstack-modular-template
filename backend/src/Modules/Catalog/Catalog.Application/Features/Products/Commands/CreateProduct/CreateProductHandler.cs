using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Catalog.Application.Features.Products.Queries.GetProduct;
using Catalog.Domain;
using Catalog.Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Commands.CreateProduct;

/// <summary>Creates a draft product. A repeat with the same slug answers 409, so a double submit cannot make two.</summary>
public sealed class CreateProductHandler(ICatalogDbContext db, TimeProvider clock)
    : ICommandHandler<CreateProductCommand, Result<ProductResponse>>
{
    public async ValueTask<Result<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var slug = Slug.Create(command.Slug);
        if (slug.IsFailure)
            return slug.Error!;

        if (!await db.Categories.AnyAsync(c => c.Id == command.CategoryId, cancellationToken))
            return ProductErrors.UnknownCategory;

        if (await db.Products.AnyAsync(p => p.Slug == slug.Value.Value, cancellationToken))
            return CatalogErrors.SlugTaken;

        var product = Product.Create(
            command.Name, slug.Value, command.Description, command.Price, command.CategoryId, clock.GetUtcNow());
        if (product.IsFailure)
            return product.Error!;

        db.Products.Add(product.Value);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return CatalogErrors.SlugTaken;
        }

        return ProductMapping.ToResponseOf(product.Value);
    }
}
