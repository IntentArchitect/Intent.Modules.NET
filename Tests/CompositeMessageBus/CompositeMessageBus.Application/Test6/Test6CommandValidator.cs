using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test6
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test6CommandValidator : AbstractValidator<Test6Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test6CommandValidator()
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