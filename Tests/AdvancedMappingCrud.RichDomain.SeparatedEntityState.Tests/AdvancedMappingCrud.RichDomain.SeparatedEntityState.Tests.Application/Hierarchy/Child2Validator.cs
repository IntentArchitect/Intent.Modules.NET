using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Child2Validator : AbstractValidator<Child2>
    {
        [IntentManaged(Mode.Merge)]
        public Child2Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ChildName)
                .NotNull();
        }
    }
}