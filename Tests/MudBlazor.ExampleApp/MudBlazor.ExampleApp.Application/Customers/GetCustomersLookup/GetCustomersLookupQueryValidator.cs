using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Customers.GetCustomersLookup
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersLookupQueryValidator : AbstractValidator<GetCustomersLookupQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersLookupQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}