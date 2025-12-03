using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CompositeMessageBus.Application.Test1
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Test1CommandValidator : AbstractValidator<Test1Command>
    {
        [IntentManaged(Mode.Merge)]
        public Test1CommandValidator()
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