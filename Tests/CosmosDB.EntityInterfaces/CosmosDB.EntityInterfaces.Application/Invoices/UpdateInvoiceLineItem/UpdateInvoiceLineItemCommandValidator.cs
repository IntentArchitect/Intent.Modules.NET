using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices.UpdateInvoiceLineItem
{
    public class UpdateInvoiceLineItemCommandValidator : AbstractValidator<UpdateInvoiceLineItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceLineItemCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}