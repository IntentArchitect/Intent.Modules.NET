using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.GetCustomersPagedFiltered
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersPagedFilteredQueryValidator : AbstractValidator<GetCustomersPagedFilteredQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedFilteredQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}