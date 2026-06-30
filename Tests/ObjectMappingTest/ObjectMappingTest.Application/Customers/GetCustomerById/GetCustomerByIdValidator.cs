using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMappingTest.Application.Customers.GetCustomerById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerByIdValidator : AbstractValidator<GetCustomerById>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerByIdValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}