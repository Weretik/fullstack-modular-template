using FluentValidation;
using Orders.Domain.Orders;

namespace Orders.Application.Features.Orders.Commands.PlaceOrder;

public sealed class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderValidator()
    {
        RuleFor(c => c.IdempotencyKey).NotEmpty().MaximumLength(100);
        RuleFor(c => c.CustomerName).NotEmpty().MaximumLength(Order.CustomerNameMaxLength);
        RuleFor(c => c.Phone).NotEmpty().Matches(@"^\+?[0-9 ()-]{7,20}$")
            .WithMessage("Enter a phone number, for example +1 555 123 4567.");
        RuleFor(c => c.Lines).NotEmpty().Must(lines => lines.Count <= 50);
        RuleFor(c => c.Lines)
            .Must(lines => lines.Select(l => l.ProductId).Distinct().Count() == lines.Count)
            .WithMessage("Each product may appear only once in the cart.");
        RuleForEach(c => c.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty();
            line.RuleFor(l => l.Quantity).InclusiveBetween(1, OrderLine.MaxQuantity);
        });
    }
}
