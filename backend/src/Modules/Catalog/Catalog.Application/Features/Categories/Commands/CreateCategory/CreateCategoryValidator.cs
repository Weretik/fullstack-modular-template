using Catalog.Domain;
using Catalog.Domain.Categories;
using FluentValidation;

namespace Catalog.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(Category.NameMaxLength);
        RuleFor(c => c.Slug).NotEmpty().MaximumLength(Slug.MaxLength);
    }
}
