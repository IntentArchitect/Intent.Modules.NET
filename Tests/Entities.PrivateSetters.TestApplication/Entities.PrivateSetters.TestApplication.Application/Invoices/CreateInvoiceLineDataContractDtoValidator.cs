using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateInvoiceLineDataContractDtoValidator : AbstractValidator<CreateInvoiceLineDataContractDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceLineDataContractDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}