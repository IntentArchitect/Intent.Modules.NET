using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandOrderItemsDtoValidator : AbstractValidator<CreateOrderCommandOrderItemsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandOrderItemsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {

            RuleFor(v => v.ProductId)
                .NotNull();
        }
    }
}