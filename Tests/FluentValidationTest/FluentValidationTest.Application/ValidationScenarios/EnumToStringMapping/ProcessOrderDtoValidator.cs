using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProcessOrderDtoValidator : AbstractValidator<ProcessOrderDto>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessOrderDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Status)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Notes)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Process)
                .NotNull()
                .IsInEnum();
        }
    }
}