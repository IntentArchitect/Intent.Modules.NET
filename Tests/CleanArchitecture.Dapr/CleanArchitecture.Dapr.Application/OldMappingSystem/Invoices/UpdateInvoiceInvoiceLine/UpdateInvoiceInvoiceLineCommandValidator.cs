using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.UpdateInvoiceInvoiceLine
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateInvoiceInvoiceLineCommandValidator : AbstractValidator<UpdateInvoiceInvoiceLineCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateInvoiceInvoiceLineCommandValidator()
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

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}