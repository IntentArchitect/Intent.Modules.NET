using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Solace.Tests.Application.Purchases.GetPurchases
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPurchasesQueryValidator : AbstractValidator<GetPurchasesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPurchasesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}