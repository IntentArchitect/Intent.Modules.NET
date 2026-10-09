using FluentValidation;

namespace CqrsSpike.Publisher.Application.Orders.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CustomerName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(v => v.Total)
                .GreaterThanOrEqualTo(0.01m);
        }
    }
}