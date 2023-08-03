using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices.GetInvoiceById
{
    public class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetInvoiceByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}