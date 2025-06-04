using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.FluentValidation.ModelDefinitionValidator", Version = "1.0")]

namespace MudBlazor.Sample.Client.Pages.Invoices.Components
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceModelValidator : AbstractValidator<InvoiceModel>
    {
        [IntentManaged(Mode.Merge)]
        public InvoiceModelValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceNo)
                .NotNull()
                .MaximumLength(12);

            RuleFor(v => v.OrderLines)
                .NotNull();

            RuleFor(v => v.CustomerName)
                .NotNull()
                .MaximumLength(40);

            RuleFor(v => v.CustomerAccountNo)
                .MaximumLength(20);

            RuleFor(v => v.AddressLine1)
                .NotNull()
                .MaximumLength(200);

            RuleFor(v => v.AddressLine2)
                .MaximumLength(200);

            RuleFor(v => v.AddressCity)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.AddressCountry)
                .NotNull()
                .MaximumLength(50);

            RuleFor(v => v.AddressPostal)
                .NotNull()
                .MaximumLength(20);

            RuleFor(v => v.Reference)
                .MaximumLength(25);
        }
    }
}