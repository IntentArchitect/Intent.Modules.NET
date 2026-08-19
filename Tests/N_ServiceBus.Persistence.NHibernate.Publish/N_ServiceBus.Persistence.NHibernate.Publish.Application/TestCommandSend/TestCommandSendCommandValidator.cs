using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace N_ServiceBus.Persistence.NHibernate.Publish.Application.TestCommandSend
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestCommandSendCommandValidator : AbstractValidator<TestCommandSendCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandSendCommandValidator()
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