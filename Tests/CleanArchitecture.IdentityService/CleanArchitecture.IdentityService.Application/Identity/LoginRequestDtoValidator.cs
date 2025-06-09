using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public LoginRequestDtoValidator()
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
        }
    }
}