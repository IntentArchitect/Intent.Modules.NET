using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
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