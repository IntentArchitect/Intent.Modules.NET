using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.CustomerId)
                .NotNull();

            RuleFor(v => v.RefNo)
                .NotNull();

            RuleFor(v => v.OrderStatus)
                .NotNull()
                .IsInEnum();
        }
    }
}