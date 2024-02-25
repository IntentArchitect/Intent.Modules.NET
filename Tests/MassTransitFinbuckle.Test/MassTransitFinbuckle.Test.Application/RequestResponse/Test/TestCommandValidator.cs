using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransitFinbuckle.Test.Application.RequestResponse.Test
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestCommandValidator : AbstractValidator<TestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}