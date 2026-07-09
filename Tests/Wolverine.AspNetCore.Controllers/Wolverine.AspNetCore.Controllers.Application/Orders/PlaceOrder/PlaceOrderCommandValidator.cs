using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.CommandValidator", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Application.PlaceOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PlaceOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}