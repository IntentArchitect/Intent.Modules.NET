using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV2
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestCommandV2Validator : AbstractValidator<TestCommandV2>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandV2Validator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}