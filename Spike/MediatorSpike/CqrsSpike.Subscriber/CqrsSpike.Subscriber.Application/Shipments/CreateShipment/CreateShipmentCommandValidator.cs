using FluentValidation;

namespace CqrsSpike.Subscriber.Application.Shipments.CreateShipment
{
    public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderId)
                .NotEmpty();
            // Implement custom validation logic here if required
        }
    }
}