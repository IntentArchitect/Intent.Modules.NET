using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SimplePayloadDtoValidator : AbstractValidator<SimplePayloadDto>
    {
        [IntentManaged(Mode.Merge)]
        public SimplePayloadDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PayloadName)
                .NotNull();
        }
    }
}