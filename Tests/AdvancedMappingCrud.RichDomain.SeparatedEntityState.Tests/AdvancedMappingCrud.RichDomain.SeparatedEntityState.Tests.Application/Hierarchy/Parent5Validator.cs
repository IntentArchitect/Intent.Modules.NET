using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Parent5Validator : AbstractValidator<Parent5>
    {
        [IntentManaged(Mode.Merge)]
        public Parent5Validator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            Include(provider.GetValidator<Grandparent5>());

            RuleFor(v => v.ParentName)
                .NotNull();

            RuleFor(v => v.Aunt)
                .NotNull()
                .SetValidator(provider.GetValidator<Aunt5>()!);
        }
    }
}