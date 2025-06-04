using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Client.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceDtoValidator : AbstractValidator<InvoiceDto>
    {
        [IntentManaged(Mode.Merge)]
        public InvoiceDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.InvoiceNo)
                .NotNull()
                .MaximumLength(12);

            RuleFor(v => v.InvoiceDate)
                .NotEmpty();

            RuleFor(v => v.OrderLines)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<InvoiceInvoiceLineDto>()!));

            RuleFor(v => v.CustomerId)
                .NotEmpty();

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

            RuleFor(v => v.DueDate)
                .NotEmpty();

            RuleFor(v => v.Reference)
                .MaximumLength(25);
        }
    }
}