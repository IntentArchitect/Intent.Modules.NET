using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.RecursiveDtos
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RecursiveNodeDtoValidator : AbstractValidator<RecursiveNodeDto>
    {
        [IntentManaged(Mode.Merge)]
        public RecursiveNodeDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Child)
                .SetValidator(this);
        }
    }
}