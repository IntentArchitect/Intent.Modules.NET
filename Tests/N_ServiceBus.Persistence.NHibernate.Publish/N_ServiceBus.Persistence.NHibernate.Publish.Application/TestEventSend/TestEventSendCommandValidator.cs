using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace N_ServiceBus.Persistence.NHibernate.Publish.Application.TestEventSend
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestEventSendCommandValidator : AbstractValidator<TestEventSendCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestEventSendCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Message)
                .NotNull();
        }
    }
}