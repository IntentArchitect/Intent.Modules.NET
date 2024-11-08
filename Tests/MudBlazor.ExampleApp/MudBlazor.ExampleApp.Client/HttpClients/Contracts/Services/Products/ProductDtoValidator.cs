using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        [IntentManaged(Mode.Merge)]
        public ProductDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(25);

            RuleFor(v => v.Description)
                .NotNull()
                .MaximumLength(100);
        }
    }
}