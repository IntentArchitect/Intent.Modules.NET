using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.CommandValidator", Version = "2.0")]

namespace CqrsSpike.Publisher.Application.Orders.MarkOrderShipped
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MarkOrderShippedCommandValidator : AbstractValidator<MarkOrderShippedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public MarkOrderShippedCommandValidator()
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