using Catalog.Domain;
using FluentValidation;

namespace Catalog.Application.Features.Storefront.Queries.GetPublishedProduct;

public sealed class GetPublishedProductValidator : AbstractValidator<GetPublishedProductQuery>
{
    public GetPublishedProductValidator() => RuleFor(q => q.Slug).NotEmpty().MaximumLength(Slug.MaxLength);
}
