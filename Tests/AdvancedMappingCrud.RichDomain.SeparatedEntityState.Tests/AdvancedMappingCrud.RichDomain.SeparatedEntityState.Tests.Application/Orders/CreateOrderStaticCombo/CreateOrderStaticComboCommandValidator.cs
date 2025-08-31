using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Orders.CreateOrderStaticCombo
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderStaticComboCommandValidator : AbstractValidator<CreateOrderStaticComboCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderStaticComboCommandValidator()
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