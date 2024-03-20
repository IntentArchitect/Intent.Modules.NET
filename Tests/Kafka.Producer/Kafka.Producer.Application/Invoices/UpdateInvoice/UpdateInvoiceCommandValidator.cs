using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Kafka.Producer.Application.Invoices.UpdateInvoice
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Note)
                .NotNull();
        }
    }
}