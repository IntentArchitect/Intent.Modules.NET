using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        [IntentManaged(Mode.Merge)]
        public RegisterDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotNull();

            RuleFor(v => v.Password)
                .NotNull();
        }
    }
}