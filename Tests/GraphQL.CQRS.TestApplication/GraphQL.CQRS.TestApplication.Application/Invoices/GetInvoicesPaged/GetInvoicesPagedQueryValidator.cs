using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesPaged
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoicesPagedQueryValidator : AbstractValidator<GetInvoicesPagedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInvoicesPagedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PageSize)
                .NotNull();
        }
    }
}