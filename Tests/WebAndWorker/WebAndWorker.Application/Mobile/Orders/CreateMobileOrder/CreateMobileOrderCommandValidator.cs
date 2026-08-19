using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace WebAndWorker.Application.Mobile.Orders.CreateMobileOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMobileOrderCommandValidator : AbstractValidator<CreateMobileOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMobileOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}