using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRoot
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteImplicitKeyAggrRootCommandValidator : AbstractValidator<DeleteImplicitKeyAggrRootCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteImplicitKeyAggrRootCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}