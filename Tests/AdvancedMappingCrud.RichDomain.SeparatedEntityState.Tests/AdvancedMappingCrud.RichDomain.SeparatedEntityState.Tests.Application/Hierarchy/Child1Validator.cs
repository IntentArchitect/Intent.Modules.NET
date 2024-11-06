using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Child1Validator : AbstractValidator<Child1>
    {
        [IntentManaged(Mode.Merge)]
        public Child1Validator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            Include(provider.GetValidator<Parent1>());

            RuleFor(v => v.ChildName)
                .NotNull();
        }
    }
}