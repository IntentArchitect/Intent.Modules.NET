using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRootImplicitKeyNestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidator : AbstractValidator<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}