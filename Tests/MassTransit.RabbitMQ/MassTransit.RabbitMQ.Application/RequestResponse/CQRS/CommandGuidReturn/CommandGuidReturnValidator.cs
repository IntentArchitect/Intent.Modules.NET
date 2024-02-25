using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.RabbitMQ.Application.RequestResponse.CQRS.CommandGuidReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandGuidReturnValidator : AbstractValidator<CommandGuidReturn>
    {
        [IntentManaged(Mode.Merge)]
        public CommandGuidReturnValidator()
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