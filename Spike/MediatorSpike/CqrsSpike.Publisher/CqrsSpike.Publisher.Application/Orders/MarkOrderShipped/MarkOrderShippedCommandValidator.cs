using FluentValidation;

namespace CqrsSpike.Publisher.Application.Orders.MarkOrderShipped
{
    public class MarkOrderShippedCommandValidator : AbstractValidator<MarkOrderShippedCommand>
    {
        public MarkOrderShippedCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}