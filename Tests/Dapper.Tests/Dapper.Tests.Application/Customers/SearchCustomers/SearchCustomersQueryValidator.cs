using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Dapper.Tests.Application.Customers.SearchCustomers
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
            RuleFor(v => v.SearchTerm)
                .NotNull();
        }
    }
}