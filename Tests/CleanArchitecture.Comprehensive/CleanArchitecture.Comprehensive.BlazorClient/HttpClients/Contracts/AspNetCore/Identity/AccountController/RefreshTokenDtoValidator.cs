using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
    {
        [IntentManaged(Mode.Merge)]
        public RefreshTokenDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefreshToken)
                .NotNull();
        }
    }
}