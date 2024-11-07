using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Parent3Validator : AbstractValidator<Parent3>
    {
        [IntentManaged(Mode.Merge)]
        public Parent3Validator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            Include(provider.GetValidator<Grandparent3>());

            RuleFor(v => v.ParentName)
                .NotNull();
        }
    }
}