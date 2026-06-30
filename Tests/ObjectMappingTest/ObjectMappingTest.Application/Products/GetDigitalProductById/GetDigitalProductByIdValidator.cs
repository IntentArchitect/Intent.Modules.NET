using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMappingTest.Application.Products.GetDigitalProductById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDigitalProductByIdValidator : AbstractValidator<GetDigitalProductById>
    {
        [IntentManaged(Mode.Merge)]
        public GetDigitalProductByIdValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}