using FluentValidation;
using TradingEngine.Application.Features.Orders.Commands;
using TradingEngine.Domain.Enums;

namespace TradingEngineApi.Validators;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.Symbol)
            .NotEmpty()
            .Matches("^[A-Z]{2,20}$");

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
