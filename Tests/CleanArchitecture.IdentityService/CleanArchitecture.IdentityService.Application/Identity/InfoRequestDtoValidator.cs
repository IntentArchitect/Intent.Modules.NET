using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InfoRequestDtoValidator : AbstractValidator<InfoRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public InfoRequestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.NewEmail)
                .NotNull()
                .EmailAddress();

            RuleFor(v => v.NewPassword)
                .NotNull();

            RuleFor(v => v.OldPassword)
                .NotNull();
        }
    }
}