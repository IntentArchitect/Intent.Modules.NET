using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateCrossFieldRules
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidateCrossFieldRulesCommandValidator : AbstractValidator<ValidateCrossFieldRulesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateCrossFieldRulesCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PrimaryValue)
                .NotNull()
                .Must(value => value == "OK");

            RuleFor(v => v.SecondaryValue)
                .NotNull();
        }
    }
}