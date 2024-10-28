using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.Versioned.TestQueryV2
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestQueryV2Validator : AbstractValidator<TestQueryV2>
    {
        [IntentManaged(Mode.Merge)]
        public TestQueryV2Validator()
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