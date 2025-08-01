using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.CreateInvoiceInvoiceLine
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateInvoiceInvoiceLineCommandValidator : AbstractValidator<CreateInvoiceInvoiceLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceInvoiceLineCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}