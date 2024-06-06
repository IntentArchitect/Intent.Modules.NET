using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Solace.Tests.Application.Purchases.GetPurchaseById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPurchaseByIdQueryValidator : AbstractValidator<GetPurchaseByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPurchaseByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}