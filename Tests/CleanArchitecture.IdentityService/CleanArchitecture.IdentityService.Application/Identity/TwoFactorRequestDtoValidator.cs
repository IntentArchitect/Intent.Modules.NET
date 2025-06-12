using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TwoFactorRequestDtoValidator : AbstractValidator<TwoFactorRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public TwoFactorRequestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ResetSharedKey)
                .NotEmpty();

            RuleFor(v => v.ResetRecoveryCodes)
                .NotEmpty();

            RuleFor(v => v.ForgetMachine)
                .NotEmpty();
        }
    }
}