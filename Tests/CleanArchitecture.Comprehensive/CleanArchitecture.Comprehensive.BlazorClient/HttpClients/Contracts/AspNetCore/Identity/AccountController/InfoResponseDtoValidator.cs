using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InfoResponseDtoValidator : AbstractValidator<InfoResponseDto>
    {
        [IntentManaged(Mode.Merge)]
        public InfoResponseDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}