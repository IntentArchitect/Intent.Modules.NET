using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.ExplicitWithReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ExplicitWithReturnCommandValidator : AbstractValidator<ExplicitWithReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ExplicitWithReturnCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}