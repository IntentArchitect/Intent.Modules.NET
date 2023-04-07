using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.UpdateEntityWithMutableOperation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEntityWithMutableOperationCommandValidator : AbstractValidator<UpdateEntityWithMutableOperationCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public UpdateEntityWithMutableOperationCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

        }
    }
}