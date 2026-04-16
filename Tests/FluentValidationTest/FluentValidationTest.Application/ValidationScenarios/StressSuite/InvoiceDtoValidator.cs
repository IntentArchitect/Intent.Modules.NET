using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceDtoValidator : AbstractValidator<InvoiceDto>
    {
        [IntentManaged(Mode.Merge)]
        public InvoiceDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceNumber)
                .NotNull();

            RuleFor(v => v.Amount)
                .GreaterThanOrEqualTo(0.01m);
        }
    }
}