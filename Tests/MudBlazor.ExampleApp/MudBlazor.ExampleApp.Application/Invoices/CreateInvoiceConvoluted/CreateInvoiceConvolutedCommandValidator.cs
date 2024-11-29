using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Application.Common.Validation;
using MudBlazor.ExampleApp.Application.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.CreateInvoiceConvoluted
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