using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.CommandValidator", Version = "2.0")]

namespace TestMediator.NoEventing.Application.Products.ChangeProductPrice
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeProductPriceCommandValidator : AbstractValidator<ChangeProductPriceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeProductPriceCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}