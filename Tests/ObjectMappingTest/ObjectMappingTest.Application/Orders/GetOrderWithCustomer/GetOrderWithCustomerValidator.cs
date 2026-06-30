using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMappingTest.Application.Orders.GetOrderWithCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderWithCustomerValidator : AbstractValidator<GetOrderWithCustomer>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderWithCustomerValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}