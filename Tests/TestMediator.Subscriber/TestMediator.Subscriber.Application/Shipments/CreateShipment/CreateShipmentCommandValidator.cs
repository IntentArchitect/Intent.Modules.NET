using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.CommandValidator", Version = "2.0")]

namespace TestMediator.Subscriber.Application.Shipments.CreateShipment
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateShipmentCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderId)
                .NotEmpty();
            // Implement custom validation logic here if required
        }
    }
}