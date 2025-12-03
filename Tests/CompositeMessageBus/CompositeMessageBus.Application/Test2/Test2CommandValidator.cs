using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test2
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test2CommandValidator : AbstractValidator<Test2Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test2CommandValidator()
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