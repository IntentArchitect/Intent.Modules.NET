using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.Versioned.TestCommandV2
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestCommandV2Validator : AbstractValidator<TestCommandV2>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandV2Validator()
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