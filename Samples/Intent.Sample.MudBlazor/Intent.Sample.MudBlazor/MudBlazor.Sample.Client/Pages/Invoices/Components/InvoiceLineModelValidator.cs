using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.FluentValidation.ModelDefinitionValidator", Version = "1.0")]

namespace MudBlazor.Sample.Client.Pages.Invoices.Components
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceLineModelValidator : AbstractValidator<InvoiceLineModel>
    {
        [IntentManaged(Mode.Merge)]
        public InvoiceLineModelValidator()
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