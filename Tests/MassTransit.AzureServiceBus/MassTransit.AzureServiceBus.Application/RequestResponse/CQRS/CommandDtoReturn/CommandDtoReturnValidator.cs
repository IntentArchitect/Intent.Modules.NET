using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandDtoReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandDtoReturnValidator : AbstractValidator<CommandDtoReturn>
    {
        [IntentManaged(Mode.Merge)]
        public CommandDtoReturnValidator()
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