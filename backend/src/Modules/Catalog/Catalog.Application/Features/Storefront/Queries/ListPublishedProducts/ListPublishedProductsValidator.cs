using BuildingBlocks.Application.Persistence;
using Catalog.Domain;
using FluentValidation;

namespace Catalog.Application.Features.Storefront.Queries.ListPublishedProducts;

public sealed class ListPublishedProductsValidator : AbstractValidator<ListPublishedProductsQuery>
{
    public ListPublishedProductsValidator()
    {
        RuleFor(q => q.CategorySlug).MaximumLength(Slug.MaxLength);
        RuleFor(q => q.Search).ValidSearch();
        RuleFor(q => q.Page).ValidPage();
        RuleFor(q => q.PageSize).ValidPageSize();
    }
}
