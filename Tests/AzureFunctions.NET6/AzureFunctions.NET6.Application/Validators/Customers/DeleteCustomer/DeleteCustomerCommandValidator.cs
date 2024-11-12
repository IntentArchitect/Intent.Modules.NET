using System;
using AzureFunctions.NET6.Application.Customers.DeleteCustomer;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Customers.DeleteCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public DeleteCustomerCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}