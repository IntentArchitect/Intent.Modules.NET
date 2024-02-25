using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandVoidReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandVoidReturnValidator : AbstractValidator<CommandVoidReturn>
    {
        [IntentManaged(Mode.Merge)]
        public CommandVoidReturnValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Input)
                .NotNull();
        }
    }
}