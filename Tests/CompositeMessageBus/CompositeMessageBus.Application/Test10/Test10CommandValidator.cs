using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test10
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test10CommandValidator : AbstractValidator<Test10Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test10CommandValidator()
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