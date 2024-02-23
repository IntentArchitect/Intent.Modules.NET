using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Invoices.GetInvoiceLineItemById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoiceLineItemByIdQueryValidator : AbstractValidator<GetInvoiceLineItemByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInvoiceLineItemByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}