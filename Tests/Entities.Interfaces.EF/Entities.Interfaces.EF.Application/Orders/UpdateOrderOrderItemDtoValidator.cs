using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateOrderOrderItemDtoValidator : AbstractValidator<UpdateOrderOrderItemDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderOrderItemDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}