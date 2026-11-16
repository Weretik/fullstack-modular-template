using Catalog.Domain;
using Catalog.Domain.Products;
using FluentValidation;

namespace Catalog.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(Product.NameMaxLength);
        RuleFor(c => c.Slug).NotEmpty().MaximumLength(Slug.MaxLength);
        RuleFor(c => c.Description).MaximumLength(Product.DescriptionMaxLength);
        RuleFor(c => c.Price).GreaterThan(0).PrecisionScale(12, 2, ignoreTrailingZeros: true);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}
