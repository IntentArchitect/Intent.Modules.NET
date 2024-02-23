using AzureFunctions.TestApplication.Application.RabbitMQTrigger.CommandForRabbitMQTrigger;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.Validators.RabbitMQTrigger.CommandForRabbitMQTrigger
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandForRabbitMQTriggerValidator : AbstractValidator<Application.RabbitMQTrigger.CommandForRabbitMQTrigger.CommandForRabbitMQTrigger>
    {
        [IntentManaged(Mode.Merge)]
        public CommandForRabbitMQTriggerValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}