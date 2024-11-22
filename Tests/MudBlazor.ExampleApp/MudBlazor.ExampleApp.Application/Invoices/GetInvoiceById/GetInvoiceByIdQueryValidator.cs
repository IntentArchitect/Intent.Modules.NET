using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.GetInvoiceById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInvoiceByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}