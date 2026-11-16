using FluentValidation;

namespace Orders.Application.Features.Orders.Queries.GetOrder;

public sealed class GetOrderValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderValidator() => RuleFor(q => q.Id).NotEmpty();
}
