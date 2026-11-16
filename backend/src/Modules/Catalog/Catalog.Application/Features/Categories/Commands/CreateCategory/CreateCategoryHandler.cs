using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Domain.Results;
using Catalog.Application.Features.Categories.Queries.ListCategories;
using Catalog.Domain;
using Catalog.Domain.Categories;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryHandler(ICatalogDbContext db)
    : ICommandHandler<CreateCategoryCommand, Result<CategoryResponse>>
{
    public async ValueTask<Result<CategoryResponse>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var slug = Slug.Create(command.Slug);
        if (slug.IsFailure)
            return slug.Error!;

        if (await db.Categories.AnyAsync(c => c.Slug == slug.Value.Value, cancellationToken))
            return CatalogErrors.SlugTaken;

        var category = Category.Create(command.Name, slug.Value);
        if (category.IsFailure)
            return category.Error!;

        db.Categories.Add(category.Value);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            // Another request took the slug between the check and the save.
            return CatalogErrors.SlugTaken;
        }

        return new CategoryResponse(category.Value.Id, category.Value.Name, category.Value.Slug);
    }
}
