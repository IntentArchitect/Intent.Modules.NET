using AzureFunctions.NET6.Application.Customers.GetCustomerThing;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Customers.GetCustomerThing
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerThingValidator : AbstractValidator<Application.Customers.GetCustomerThing.GetCustomerThing>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerThingValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}