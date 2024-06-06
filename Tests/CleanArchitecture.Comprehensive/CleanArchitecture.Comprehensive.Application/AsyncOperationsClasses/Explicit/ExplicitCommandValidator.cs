using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.Explicit
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ExplicitCommandValidator : AbstractValidator<ExplicitCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ExplicitCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}