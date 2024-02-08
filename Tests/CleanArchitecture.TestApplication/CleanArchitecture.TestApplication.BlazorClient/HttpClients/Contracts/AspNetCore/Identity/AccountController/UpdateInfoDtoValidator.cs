using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class UpdateInfoDtoValidator : AbstractValidator<UpdateInfoDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateInfoDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.NewEmail)
                .NotNull();

            RuleFor(v => v.NewPassword)
                .NotNull();

            RuleFor(v => v.OldPassword)
                .NotNull();
        }
    }
}