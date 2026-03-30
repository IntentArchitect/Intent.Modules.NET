using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Queries.ValidateExplicitQueryRules
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidateExplicitQueryRulesQueryValidator : AbstractValidator<ValidateExplicitQueryRulesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateExplicitQueryRulesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RequiredFilter)
                .NotNull()
                .NotEmpty();

            RuleFor(v => v.PageNo)
                .GreaterThanOrEqualTo(1);

            RuleFor(v => v.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}