using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public RegisterRequestDtoValidator()
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