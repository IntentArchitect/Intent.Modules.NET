using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OptionalSuppliedDtoValidator : AbstractValidator<OptionalSuppliedDto>
    {
        [IntentManaged(Mode.Merge)]
        public OptionalSuppliedDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}