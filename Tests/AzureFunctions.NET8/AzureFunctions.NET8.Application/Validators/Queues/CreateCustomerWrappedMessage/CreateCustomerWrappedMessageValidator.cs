using System;
using AzureFunctions.NET8.Application.Queues.CreateCustomerWrappedMessage;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.Queues.CreateCustomerWrappedMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerWrappedMessageValidator : AbstractValidator<Application.Queues.CreateCustomerWrappedMessage.CreateCustomerWrappedMessage>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateCustomerWrappedMessageValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}