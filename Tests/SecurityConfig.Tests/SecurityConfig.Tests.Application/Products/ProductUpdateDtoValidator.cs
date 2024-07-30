using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace SecurityConfig.Tests.Application.Products
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public ProductUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();
        }
    }
}