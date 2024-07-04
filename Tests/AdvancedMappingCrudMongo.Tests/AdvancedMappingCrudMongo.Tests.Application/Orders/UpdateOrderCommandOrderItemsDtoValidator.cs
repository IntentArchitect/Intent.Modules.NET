using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateOrderCommandOrderItemsDtoValidator : AbstractValidator<UpdateOrderCommandOrderItemsDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderCommandOrderItemsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.ProductId)
                .NotNull();
        }
    }
}