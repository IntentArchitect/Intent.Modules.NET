using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ConfirmEmailDtoValidator : AbstractValidator<ConfirmEmailDto>
    {
        [IntentManaged(Mode.Merge)]
        public ConfirmEmailDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.UserId)
                .NotNull();

            RuleFor(v => v.Code)
                .NotNull();
        }
    }
}