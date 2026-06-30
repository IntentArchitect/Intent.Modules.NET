using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMappingTest.Application.Customers.GetCustomerDetail
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerDetailValidator : AbstractValidator<GetCustomerDetail>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerDetailValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}