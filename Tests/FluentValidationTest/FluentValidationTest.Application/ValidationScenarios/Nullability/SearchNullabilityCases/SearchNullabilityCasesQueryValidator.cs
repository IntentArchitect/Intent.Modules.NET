using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability.SearchNullabilityCases
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SearchNullabilityCasesQueryValidator : AbstractValidator<SearchNullabilityCasesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public SearchNullabilityCasesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.TenantId)
                .Must(value => value != Guid.Empty);

            RuleFor(v => v.Category)
                .MinimumLength(3);

            RuleFor(v => v.MinValue)
                .GreaterThanOrEqualTo(0);

            RuleFor(v => v.PageNo)
                .GreaterThanOrEqualTo(1);

            RuleFor(v => v.PageSize)
                .LessThanOrEqualTo(100);
        }
    }
}