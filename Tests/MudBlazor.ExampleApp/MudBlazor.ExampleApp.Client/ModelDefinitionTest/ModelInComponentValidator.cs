using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.ModelDefinitionTest;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.FluentValidation.ModelDefinitionValidator", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ModelInComponentValidator : AbstractValidator<ModelDefinitionInComponent.ModelInComponent>
    {
        [IntentManaged(Mode.Merge)]
        public ModelInComponentValidator()
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
            ValidationContext<ModelDefinitionInComponent.ModelInComponent> validationContext,
            CancellationToken cancellationToken)
        {
            // TODO: Implement ValidatePropertyAsync (ModelInComponentValidator) functionality
            throw new NotImplementedException("Your custom validation rules here...");
        }
    }
}