using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test3
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test3CommandValidator : AbstractValidator<Test3Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test3CommandValidator()
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