using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceInvoiceLineDtoValidator : AbstractValidator<InvoiceInvoiceLineDto>
    {
        [IntentManaged(Mode.Merge)]
        public InvoiceInvoiceLineDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ProductName)
                .NotNull()
                .MaximumLength(25);

            RuleFor(v => v.ProductDescription)
                .NotNull()
                .MaximumLength(100);
        }
    }
}