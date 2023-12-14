using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.UpdateOrderOrderItem
{
    public class UpdateOrderOrderItemCommandValidator : AbstractValidator<UpdateOrderOrderItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderOrderItemCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Quantity)
                .NotNull();

            RuleFor(v => v.ProductId)
                .NotNull();
        }
    }
}