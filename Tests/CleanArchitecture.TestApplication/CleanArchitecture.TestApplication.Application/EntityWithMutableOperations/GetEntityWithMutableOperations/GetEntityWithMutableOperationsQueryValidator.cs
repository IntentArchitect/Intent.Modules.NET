using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.GetEntityWithMutableOperations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityWithMutableOperationsQueryValidator : AbstractValidator<GetEntityWithMutableOperationsQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetEntityWithMutableOperationsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}