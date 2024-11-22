using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Customers.DeleteCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCustomerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}