using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.CommandValidator", Version = "2.0")]

namespace WolverineEventing.Publish.RabbitMQ.Application.Orders.RequestOrderProcessing
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RequestOrderProcessingCommandValidator : AbstractValidator<RequestOrderProcessingCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RequestOrderProcessingCommandValidator()
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