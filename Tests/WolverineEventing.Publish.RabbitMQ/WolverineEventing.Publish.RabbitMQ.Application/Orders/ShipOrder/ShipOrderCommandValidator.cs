using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.CommandValidator", Version = "2.0")]

namespace WolverineEventing.Publish.RabbitMQ.Application.Orders.ShipOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ShipOrderCommandValidator : AbstractValidator<ShipOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ShipOrderCommandValidator()
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