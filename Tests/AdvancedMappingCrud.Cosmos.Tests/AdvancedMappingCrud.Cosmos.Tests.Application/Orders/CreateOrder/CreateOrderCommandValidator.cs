using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CustomerId)
                .NotNull();

            RuleFor(v => v.RefNo)
                .NotNull();

            RuleFor(v => v.OrderStatus)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.OrderTags)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOrderCommandOrderTagsDto>()!));

            RuleFor(v => v.OrderItems)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOrderCommandOrderItemsDto>()!));
        }
    }
}