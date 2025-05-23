using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Customers.GetCustomersPaged
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersPagedQueryValidator : AbstractValidator<GetCustomersPagedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}