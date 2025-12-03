using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test5
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test5CommandValidator : AbstractValidator<Test5Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test5CommandValidator()
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