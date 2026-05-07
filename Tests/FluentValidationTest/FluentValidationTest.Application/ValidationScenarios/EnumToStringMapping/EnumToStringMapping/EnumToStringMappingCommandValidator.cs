using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping.EnumToStringMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EnumToStringMappingCommandValidator : AbstractValidator<EnumToStringMappingCommand>
    {
        [IntentManaged(Mode.Merge)]
        public EnumToStringMappingCommandValidator()
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