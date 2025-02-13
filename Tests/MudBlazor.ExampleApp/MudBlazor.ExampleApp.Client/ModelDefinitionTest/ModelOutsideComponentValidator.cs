using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.FluentValidation.ModelDefinitionValidator", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.ModelDefinitionTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ModelOutsideComponentValidator : AbstractValidator<ModelOutsideComponent>
    {
        [IntentManaged(Mode.Merge)]
        public ModelOutsideComponentValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Property)
                .NotNull()
                .NotEmpty()
                .CustomAsync(ValidatePropertyAsync);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task ValidatePropertyAsync(
            string value,
            ValidationContext<ModelOutsideComponent> validationContext,
            CancellationToken cancellationToken)
        {
            // TODO: Implement ValidatePropertyAsync (ModelOutsideComponentValidator) functionality
            throw new NotImplementedException("Your custom validation rules here...");
        }
    }
}