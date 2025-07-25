using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace TableStorage.Tests.Application.Invoices.GetPagedInvoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPagedInvoicesQueryValidator : AbstractValidator<GetPagedInvoicesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPagedInvoicesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PartitionKey)
                .NotNull();
        }
    }
}