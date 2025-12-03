using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test4
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test4CommandValidator : AbstractValidator<Test4Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test4CommandValidator()
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