using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RefreshRequestDtoValidator : AbstractValidator<RefreshRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public RefreshRequestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefreshToken)
                .NotNull()
                .NotEmpty();
        }
    }
}