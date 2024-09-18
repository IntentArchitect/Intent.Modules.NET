using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.Invoices.GetInvoiceLineItems
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoiceLineItemsQueryValidator : AbstractValidator<GetInvoiceLineItemsQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetInvoiceLineItemsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();
        }
    }
}