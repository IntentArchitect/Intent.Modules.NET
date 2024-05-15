using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.DeleteInvoiceInvoiceLine
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteInvoiceInvoiceLineCommandValidator : AbstractValidator<DeleteInvoiceInvoiceLineCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public DeleteInvoiceInvoiceLineCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}