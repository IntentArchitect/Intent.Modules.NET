using System;
using AzureFunctions.NET8.Application.Queues.CreateCustomerMessage;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.Queues.CreateCustomerMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerMessageValidator : AbstractValidator<Application.Queues.CreateCustomerMessage.CreateCustomerMessage>
    {
        public CreateCustomerMessageValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}