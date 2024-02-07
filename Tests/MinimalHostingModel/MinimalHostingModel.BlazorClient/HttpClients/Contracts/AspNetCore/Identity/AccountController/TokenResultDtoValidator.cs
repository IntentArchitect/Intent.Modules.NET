using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class TokenResultDtoValidator : AbstractValidator<TokenResultDto>
    {
        [IntentManaged(Mode.Merge)]
        public TokenResultDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.TokenType)
                .NotNull();

            RuleFor(v => v.AuthenticationToken)
                .NotNull();

            RuleFor(v => v.RefreshToken)
                .NotNull();
        }
    }
}