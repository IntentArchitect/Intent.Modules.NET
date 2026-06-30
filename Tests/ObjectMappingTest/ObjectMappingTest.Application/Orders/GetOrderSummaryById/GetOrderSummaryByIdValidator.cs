using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMappingTest.Application.Orders.GetOrderSummaryById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderSummaryByIdValidator : AbstractValidator<GetOrderSummaryById>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderSummaryByIdValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}