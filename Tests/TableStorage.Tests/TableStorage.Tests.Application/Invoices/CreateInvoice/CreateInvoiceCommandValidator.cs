using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TableStorage.Tests.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PartitionKey)
                .NotNull();

            RuleFor(v => v.RowKey)
                .NotNull();

            RuleFor(v => v.OrderPartitionKey)
                .NotNull();

            RuleFor(v => v.OrderRowKey)
                .NotNull();
        }
    }
}