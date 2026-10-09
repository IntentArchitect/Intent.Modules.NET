using FluentValidation;

namespace CqrsSpike.Publisher.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CustomerName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}