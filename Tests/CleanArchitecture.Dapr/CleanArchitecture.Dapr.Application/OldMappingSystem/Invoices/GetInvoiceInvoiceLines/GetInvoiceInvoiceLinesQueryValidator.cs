using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLines
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoiceInvoiceLinesQueryValidator : AbstractValidator<GetInvoiceInvoiceLinesQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetInvoiceInvoiceLinesQueryValidator()
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