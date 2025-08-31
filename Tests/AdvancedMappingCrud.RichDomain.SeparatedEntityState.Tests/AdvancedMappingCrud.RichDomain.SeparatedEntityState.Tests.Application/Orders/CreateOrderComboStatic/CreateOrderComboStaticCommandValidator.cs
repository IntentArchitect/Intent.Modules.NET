using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Orders.CreateOrderComboStatic
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderComboStaticCommandValidator : AbstractValidator<CreateOrderComboStaticCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderComboStaticCommandValidator()
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