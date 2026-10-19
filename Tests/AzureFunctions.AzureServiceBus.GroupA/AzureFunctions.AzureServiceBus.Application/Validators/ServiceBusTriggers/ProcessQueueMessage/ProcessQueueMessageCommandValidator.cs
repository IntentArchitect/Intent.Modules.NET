using AzureFunctions.AzureServiceBus.Application.ServiceBusTriggers.ProcessQueueMessage;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Application.Validators.ServiceBusTriggers.ProcessQueueMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProcessQueueMessageCommandValidator : AbstractValidator<ProcessQueueMessageCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessQueueMessageCommandValidator()
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