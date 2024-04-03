using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Kafka.Producer.Application.Invoices.DeleteInvoice
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteInvoiceCommandValidator : AbstractValidator<DeleteInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}