using System;
using FluentValidation;
using GraphQL.AzureFunction.TestApplication.Application.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Validators.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CustomerCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.LastName)
                .NotNull();
        }
    }
}