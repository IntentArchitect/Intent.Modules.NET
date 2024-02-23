using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.GetOrderOrderItemById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderOrderItemByIdQueryValidator : AbstractValidator<GetOrderOrderItemByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderOrderItemByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}