using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.CheckResult
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CheckResultCommandValidator : AbstractValidator<CheckResultCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CheckResultCommandValidator()
        {
            ConfigureValidationRules();

        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}