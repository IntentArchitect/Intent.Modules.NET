using FluentValidation;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public OrderCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefNo)
                .NotNull();

            RuleFor(v => v.OrderItems)
                .NotNull();
        }
    }
}