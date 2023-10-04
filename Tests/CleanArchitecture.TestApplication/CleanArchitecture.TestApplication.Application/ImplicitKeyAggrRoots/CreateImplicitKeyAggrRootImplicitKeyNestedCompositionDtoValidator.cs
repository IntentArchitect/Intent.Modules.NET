using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDtoValidator : AbstractValidator<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDtoValidator()
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