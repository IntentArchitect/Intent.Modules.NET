using System;
using AzureFunctions.NET6.Application.Customers;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        [IntentManaged(Mode.Merge)]
        public CustomerDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}