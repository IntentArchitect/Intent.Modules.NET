using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.CommandValidator", Version = "2.0")]

namespace TestMediator.Subscriber.Application.Shipments.CompleteShipment
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompleteShipmentCommandValidator : AbstractValidator<CompleteShipmentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CompleteShipmentCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}