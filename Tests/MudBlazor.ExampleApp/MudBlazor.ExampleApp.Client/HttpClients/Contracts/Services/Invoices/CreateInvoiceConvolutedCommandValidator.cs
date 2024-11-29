using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.Common.Validation;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateInvoiceConvolutedCommandValidator : AbstractValidator<CreateInvoiceConvolutedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceConvolutedCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Invoice)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateInvoiceDTO>()!);
        }
    }
}