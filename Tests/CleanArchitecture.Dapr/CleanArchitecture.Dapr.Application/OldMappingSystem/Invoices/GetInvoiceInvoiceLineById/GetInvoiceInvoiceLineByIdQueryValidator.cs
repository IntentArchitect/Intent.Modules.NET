using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLineById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoiceInvoiceLineByIdQueryValidator : AbstractValidator<GetInvoiceInvoiceLineByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInvoiceInvoiceLineByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}