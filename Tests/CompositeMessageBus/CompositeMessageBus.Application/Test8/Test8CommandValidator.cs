using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test8
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test8CommandValidator : AbstractValidator<Test8Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test8CommandValidator()
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