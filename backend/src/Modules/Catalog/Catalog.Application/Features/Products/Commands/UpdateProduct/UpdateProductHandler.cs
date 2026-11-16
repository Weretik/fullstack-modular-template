using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Catalog.Application.Features.Products.Queries.GetProduct;
using Catalog.Domain;
using Catalog.Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Commands.UpdateProduct;

/// <summary>Replaces the editable data of a product. Repeating the same update gives the same product.</summary>
public sealed class UpdateProductHandler(ICatalogDbContext db, TimeProvider clock)
    : ICommandHandler<UpdateProductCommand, Result<ProductResponse>>
{
    public async ValueTask<Result<ProductResponse>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var slug = Slug.Create(command.Slug);
        if (slug.IsFailure)
            return slug.Error!;

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
        if (product is null)
            return ProductErrors.NotFound;

        if (!await db.Categories.AnyAsync(c => c.Id == command.CategoryId, cancellationToken))
            return ProductErrors.UnknownCategory;

        if (await db.Products.AnyAsync(p => p.Slug == slug.Value.Value && p.Id != command.Id, cancellationToken))
            return CatalogErrors.SlugTaken;

        var updated = product.Update(
            command.Name, slug.Value, command.Description, command.Price, command.CategoryId, command.IsPublished,
            clock.GetUtcNow());
        if (updated.IsFailure)
            return updated.Error!;

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return CatalogErrors.SlugTaken;
        }

        return ProductMapping.ToResponseOf(product);
    }
}
