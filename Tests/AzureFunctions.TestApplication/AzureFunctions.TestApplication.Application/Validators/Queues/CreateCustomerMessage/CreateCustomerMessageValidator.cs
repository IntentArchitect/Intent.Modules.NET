using System;
using AzureFunctions.TestApplication.Application.Queues.CreateCustomerMessage;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Validators.Queues.CreateCustomerMessage
{
    public class CreateCustomerMessageValidator : AbstractValidator<Application.Queues.CreateCustomerMessage.CreateCustomerMessage>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public CreateCustomerMessageValidator()
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