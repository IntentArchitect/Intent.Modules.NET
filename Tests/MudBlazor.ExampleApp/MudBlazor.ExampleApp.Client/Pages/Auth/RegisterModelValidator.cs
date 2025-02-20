using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.FluentValidation.ModelDefinitionValidator", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Auth
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        [IntentManaged(Mode.Merge)]
        public RegisterModelValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(v => v.Password)
                .NotNull()
                .NotEmpty();

            RuleFor(v => v.ConfirmPassword)
                .NotNull()
                .NotEmpty()
                .Equal(x => x.Password);
        }
    }
}