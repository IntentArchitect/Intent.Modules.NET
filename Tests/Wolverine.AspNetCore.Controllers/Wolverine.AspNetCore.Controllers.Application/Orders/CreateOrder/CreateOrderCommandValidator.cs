using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.CommandValidator", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Application.CreateOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderNumber)
                .NotNull();

            RuleFor(v => v.CustomerName)
                .NotNull();

            RuleFor(v => v.Status)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.ShippingLine1)
                .NotNull();

            RuleFor(v => v.ShippingCity)
                .NotNull();

            RuleFor(v => v.ShippingPostalCode)
                .NotNull();

            RuleFor(v => v.ShippingCountry)
                .NotNull();
        }
    }
}