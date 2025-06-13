using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.IdentityService.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ForgotPasswordRequestDtoValidator : AbstractValidator<ForgotPasswordRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public ForgotPasswordRequestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();
        }
    }
}