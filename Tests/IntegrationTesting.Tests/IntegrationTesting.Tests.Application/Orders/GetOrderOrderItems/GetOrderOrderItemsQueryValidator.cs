using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Orders.GetOrderOrderItems
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderOrderItemsQueryValidator : AbstractValidator<GetOrderOrderItemsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderOrderItemsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}