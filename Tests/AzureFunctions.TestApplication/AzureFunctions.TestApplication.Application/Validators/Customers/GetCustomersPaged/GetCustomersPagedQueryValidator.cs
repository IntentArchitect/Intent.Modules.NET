using System;
using AzureFunctions.TestApplication.Application.Customers.GetCustomersPaged;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Validators.Customers.GetCustomersPaged
{
    public class GetCustomersPagedQueryValidator : AbstractValidator<GetCustomersPagedQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetCustomersPagedQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}