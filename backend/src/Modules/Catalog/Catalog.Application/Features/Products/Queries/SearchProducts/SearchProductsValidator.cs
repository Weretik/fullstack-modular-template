using BuildingBlocks.Application.Persistence;
using FluentValidation;

namespace Catalog.Application.Features.Products.Queries.SearchProducts;

public sealed class SearchProductsValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsValidator()
    {
        RuleFor(q => q.Search).ValidSearch();
        RuleFor(q => q.Page).ValidPage();
        RuleFor(q => q.PageSize).ValidPageSize();
    }
}
