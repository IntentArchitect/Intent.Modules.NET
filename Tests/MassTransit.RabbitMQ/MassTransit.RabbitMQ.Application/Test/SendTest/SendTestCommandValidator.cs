using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.RabbitMQ.Application.Test.SendTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SendTestCommandValidator : AbstractValidator<SendTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SendTestCommandValidator()
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