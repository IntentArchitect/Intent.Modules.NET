using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.Unversioned.Test
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestQueryValidator : AbstractValidator<TestQuery>
    {
        [IntentManaged(Mode.Merge)]
        public TestQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}