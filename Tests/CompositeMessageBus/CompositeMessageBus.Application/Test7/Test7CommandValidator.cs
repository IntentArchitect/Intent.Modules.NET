using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test7
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test7CommandValidator : AbstractValidator<Test7Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test7CommandValidator()
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