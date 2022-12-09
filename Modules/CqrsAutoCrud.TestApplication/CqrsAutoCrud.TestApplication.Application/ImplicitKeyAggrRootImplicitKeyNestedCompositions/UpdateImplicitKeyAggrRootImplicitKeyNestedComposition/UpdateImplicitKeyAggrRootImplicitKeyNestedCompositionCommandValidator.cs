using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidator : AbstractValidator<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>
    {
        [IntentManaged(Mode.Fully)]
        public UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();

        }
    }
}