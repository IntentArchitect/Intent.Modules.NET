using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.RequestResponse.Client.Application.TestCommands.AzureServiceBusTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AzureServiceBusTestCommandValidator : AbstractValidator<AzureServiceBusTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public AzureServiceBusTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Message)
                .NotNull();
        }
    }
}