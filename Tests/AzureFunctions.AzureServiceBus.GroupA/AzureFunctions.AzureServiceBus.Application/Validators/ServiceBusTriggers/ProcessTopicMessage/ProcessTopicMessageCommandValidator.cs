using AzureFunctions.AzureServiceBus.Application.ServiceBusTriggers.ProcessTopicMessage;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Application.Validators.ServiceBusTriggers.ProcessTopicMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProcessTopicMessageCommandValidator : AbstractValidator<ProcessTopicMessageCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessTopicMessageCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Message)
                .NotNull();
        }
    }
}