using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.Orders.UpdateOrderOrderItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
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

            RuleFor(v => v.Description)
                .NotNull();

            RuleFor(v => v.WarehouseId)
                .NotNull();
        }
    }
}