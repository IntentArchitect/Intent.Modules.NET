using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace TableStorage.Tests.Application.Invoices.GetInvoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInvoicesQueryValidator : AbstractValidator<GetInvoicesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInvoicesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}