using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GreatGrandparent5Validator : AbstractValidator<GreatGrandparent5>
    {
        [IntentManaged(Mode.Merge)]
        public GreatGrandparent5Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.GreatGrandParentName)
                .NotNull();
        }
    }
}