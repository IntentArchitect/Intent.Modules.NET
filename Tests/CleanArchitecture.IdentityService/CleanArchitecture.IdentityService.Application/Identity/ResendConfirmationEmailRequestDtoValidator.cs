using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.IdentityService.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ResendConfirmationEmailRequestDtoValidator : AbstractValidator<ResendConfirmationEmailRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public ResendConfirmationEmailRequestDtoValidator()
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