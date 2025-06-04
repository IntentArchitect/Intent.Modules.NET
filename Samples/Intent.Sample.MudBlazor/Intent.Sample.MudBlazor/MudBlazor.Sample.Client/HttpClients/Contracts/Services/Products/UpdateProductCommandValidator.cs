using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Products
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateProductCommandValidator()
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

            RuleFor(v => v.Price)
                .GreaterThanOrEqualTo(0);
        }
    }
}