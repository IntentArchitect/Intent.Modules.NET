using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.CommandValidator", Version = "2.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Application.Products.UpdateProductPrice
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateProductPriceCommandValidator()
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