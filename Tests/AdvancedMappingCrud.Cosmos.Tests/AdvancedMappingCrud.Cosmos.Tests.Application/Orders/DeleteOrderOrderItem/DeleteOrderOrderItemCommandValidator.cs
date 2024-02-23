using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.DeleteOrderOrderItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteOrderOrderItemCommandValidator : AbstractValidator<DeleteOrderOrderItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteOrderOrderItemCommandValidator()
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