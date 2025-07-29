using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace EntityFrameworkCore.SQLLite.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefNo)
                .NotNull();
        }
    }
}