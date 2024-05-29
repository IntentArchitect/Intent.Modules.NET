using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.OperationsClasses.Sync
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SyncValidator : AbstractValidator<Sync>
    {
        [IntentManaged(Mode.Merge)]
        public SyncValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}