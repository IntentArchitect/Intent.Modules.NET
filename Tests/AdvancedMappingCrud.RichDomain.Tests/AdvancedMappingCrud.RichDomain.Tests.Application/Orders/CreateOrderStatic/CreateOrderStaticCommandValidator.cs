using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Orders.CreateOrderStatic
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderStaticCommandValidator : AbstractValidator<CreateOrderStaticCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderStaticCommandValidator()
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