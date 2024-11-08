using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateInvoiceCommandOrderLinesDtoValidator : AbstractValidator<CreateInvoiceCommandOrderLinesDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceCommandOrderLinesDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}