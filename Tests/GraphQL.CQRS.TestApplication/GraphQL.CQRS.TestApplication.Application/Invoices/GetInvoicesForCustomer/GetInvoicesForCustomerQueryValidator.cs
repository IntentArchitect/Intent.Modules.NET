using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesForCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoicesForCustomerQueryValidator : AbstractValidator<GetInvoicesForCustomerQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetInvoicesForCustomerQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}