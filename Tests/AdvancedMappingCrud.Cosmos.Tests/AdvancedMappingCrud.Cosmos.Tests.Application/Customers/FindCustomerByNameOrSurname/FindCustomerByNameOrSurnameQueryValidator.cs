using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.FindCustomerByNameOrSurname
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FindCustomerByNameOrSurnameQueryValidator : AbstractValidator<FindCustomerByNameOrSurnameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public FindCustomerByNameOrSurnameQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}