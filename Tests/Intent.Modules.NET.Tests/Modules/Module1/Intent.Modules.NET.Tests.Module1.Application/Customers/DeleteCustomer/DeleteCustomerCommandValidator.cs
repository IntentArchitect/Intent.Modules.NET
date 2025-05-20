using FluentValidation;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers.DeleteCustomer;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Customers.DeleteCustomer
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