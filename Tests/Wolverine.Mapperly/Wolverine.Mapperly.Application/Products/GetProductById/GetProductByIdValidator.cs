using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.QueryValidator", Version = "2.0")]

namespace Wolverine.Mapperly.Application.Products.GetProductById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductByIdValidator : AbstractValidator<GetProductById>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductByIdValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}