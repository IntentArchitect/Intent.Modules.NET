using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.CreateInvoice
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ClientId)
                .NotNull();
        }
    }
}