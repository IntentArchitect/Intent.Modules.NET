using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Products
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateInvoiceDTOValidator : AbstractValidator<CreateInvoiceDTO>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceDTOValidator()
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
        }
    }
}