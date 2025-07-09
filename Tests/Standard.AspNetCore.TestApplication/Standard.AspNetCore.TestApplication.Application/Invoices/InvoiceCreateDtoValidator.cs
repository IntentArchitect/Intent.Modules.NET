using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceCreateDtoValidator : AbstractValidator<InvoiceCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public InvoiceCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Number)
                .NotNull();
        }
    }
}