using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace WebAndWorker.Application.App.Orders.CreateAppOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAppOrderCommandValidator : AbstractValidator<CreateAppOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAppOrderCommandValidator()
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