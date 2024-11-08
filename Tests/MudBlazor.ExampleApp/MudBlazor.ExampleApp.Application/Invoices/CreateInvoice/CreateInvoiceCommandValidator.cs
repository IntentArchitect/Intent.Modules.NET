using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.CreateInvoice
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceNo)
                .NotNull()
                .MaximumLength(12);

            RuleFor(v => v.Reference)
                .MaximumLength(25);

            RuleFor(v => v.OrderLines)
                .NotNull();
        }
    }
}