using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateExplicitNumericRules
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidateExplicitNumericRulesCommandValidator : AbstractValidator<ValidateExplicitNumericRulesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateExplicitNumericRulesCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MinimumInt)
                .GreaterThanOrEqualTo(1);

            RuleFor(v => v.MaximumInt)
                .LessThanOrEqualTo(100);

            RuleFor(v => v.BoundedLong)
                .InclusiveBetween(10, 9999);

            RuleFor(v => v.BoundedDecimal)
                .InclusiveBetween(1m, 5000m);
        }
    }
}