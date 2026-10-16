using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.CommandValidator", Version = "2.0")]

namespace TestMediator.WolverineEventing.Application.Orders.AcceptOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AcceptOrderCommandValidator : AbstractValidator<AcceptOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public AcceptOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CustomerName)
                .NotNull();
        }
    }
}