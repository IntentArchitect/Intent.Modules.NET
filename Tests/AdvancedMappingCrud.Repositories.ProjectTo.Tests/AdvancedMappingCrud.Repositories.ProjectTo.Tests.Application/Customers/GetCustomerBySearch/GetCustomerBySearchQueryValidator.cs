using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.GetCustomerBySearch
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerBySearchQueryValidator : AbstractValidator<GetCustomerBySearchQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerBySearchQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}