using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLines
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoiceInvoiceLinesQueryValidator : AbstractValidator<GetInvoiceInvoiceLinesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInvoiceInvoiceLinesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();
        }
    }
}