using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Versioned.TestQueryV1
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestQueryV1Validator : AbstractValidator<TestQueryV1>
    {
        [IntentManaged(Mode.Merge)]
        public TestQueryV1Validator()
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