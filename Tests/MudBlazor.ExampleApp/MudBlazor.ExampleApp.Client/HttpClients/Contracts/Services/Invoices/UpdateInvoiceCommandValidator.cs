using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.InvoiceNo)
                .NotNull()
                .MaximumLength(12);

            RuleFor(v => v.Reference)
                .MaximumLength(25);

            RuleFor(v => v.OrderLines)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateInvoiceCommandOrderLinesDto>()!));
        }
    }
}