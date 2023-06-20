using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Invoices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class InvoiceUpdateDtoValidator : AbstractValidator<InvoiceUpdateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public InvoiceUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Number)
                .NotNull();

        }
    }
}