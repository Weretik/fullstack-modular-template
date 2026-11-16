using FluentValidation;

namespace Catalog.Application.Features.Products.Queries.GetProduct;

public sealed class GetProductValidator : AbstractValidator<GetProductQuery>
{
    public GetProductValidator() => RuleFor(q => q.Id).NotEmpty();
}
