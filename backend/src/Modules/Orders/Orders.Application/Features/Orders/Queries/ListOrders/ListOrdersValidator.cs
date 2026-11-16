using BuildingBlocks.Application.Persistence;
using FluentValidation;

namespace Orders.Application.Features.Orders.Queries.ListOrders;

public sealed class ListOrdersValidator : AbstractValidator<ListOrdersQuery>
{
    public ListOrdersValidator()
    {
        RuleFor(q => q.Status).IsInEnum();
        RuleFor(q => q.Page).ValidPage();
        RuleFor(q => q.PageSize).ValidPageSize();
    }
}
