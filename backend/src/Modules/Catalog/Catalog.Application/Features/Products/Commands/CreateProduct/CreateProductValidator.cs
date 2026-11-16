using Catalog.Domain;
using Catalog.Domain.Products;
using FluentValidation;

namespace Catalog.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(Product.NameMaxLength);
        RuleFor(c => c.Slug).NotEmpty().MaximumLength(Slug.MaxLength);
        RuleFor(c => c.Description).MaximumLength(Product.DescriptionMaxLength);
        RuleFor(c => c.Price).GreaterThan(0).PrecisionScale(12, 2, ignoreTrailingZeros: true);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}
