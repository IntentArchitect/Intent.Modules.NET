using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.GetOrders
{
    public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrdersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}