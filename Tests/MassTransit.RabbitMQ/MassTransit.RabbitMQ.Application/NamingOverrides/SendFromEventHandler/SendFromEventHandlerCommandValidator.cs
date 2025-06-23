using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.RabbitMQ.Application.NamingOverrides.SendFromEventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SendFromEventHandlerCommandValidator : AbstractValidator<SendFromEventHandlerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SendFromEventHandlerCommandValidator()
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