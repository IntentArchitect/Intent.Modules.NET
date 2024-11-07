using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Grandparent3Validator : AbstractValidator<Grandparent3>
    {
        [IntentManaged(Mode.Merge)]
        public Grandparent3Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.GrandparentName)
                .NotNull();
        }
    }
}