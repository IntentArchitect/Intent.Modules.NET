using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Orders.GetOrderOrderItems
{
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