using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Oracle.TestApplication.Application.Invoices.GetInvoices
{
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