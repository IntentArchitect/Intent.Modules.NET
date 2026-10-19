using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.QueryValidator", Version = "2.0")]

namespace TestMediator.NoEventing.Application.Products.GetProductPriceHistory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductPriceHistoryQueryValidator : AbstractValidator<GetProductPriceHistoryQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductPriceHistoryQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}