using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.SearchCustomers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SearchCustomersQueryValidator : AbstractValidator<SearchCustomersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public SearchCustomersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}