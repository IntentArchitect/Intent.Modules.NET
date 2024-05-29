using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.OperationsClasses.SyncWithReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SyncWithReturnValidator : AbstractValidator<SyncWithReturn>
    {
        [IntentManaged(Mode.Merge)]
        public SyncWithReturnValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}