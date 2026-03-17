using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.RequestResponse.Client.Application.TestCommands.RabbitMqTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RabbitMqTestCommandValidator : AbstractValidator<RabbitMqTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RabbitMqTestCommandValidator()
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