using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Orders.UpdateOrderItemsOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateOrderItemsOrderCommandValidator : AbstractValidator<UpdateOrderItemsOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderItemsOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderItemDetails)
                .NotNull();
        }
    }
}