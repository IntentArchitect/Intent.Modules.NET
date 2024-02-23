using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices.DeleteInvoiceLineItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteInvoiceLineItemCommandValidator : AbstractValidator<DeleteInvoiceLineItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteInvoiceLineItemCommandValidator()
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