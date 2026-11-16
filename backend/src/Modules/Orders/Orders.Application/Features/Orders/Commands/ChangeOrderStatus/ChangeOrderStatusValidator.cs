using FluentValidation;

namespace Orders.Application.Features.Orders.Commands.ChangeOrderStatus;

public sealed class ChangeOrderStatusValidator : AbstractValidator<ChangeOrderStatusCommand>
{
    public ChangeOrderStatusValidator()
    {
        RuleFor(c => c.OrderId).NotEmpty();
        RuleFor(c => c.Status).IsInEnum();
    }
}
