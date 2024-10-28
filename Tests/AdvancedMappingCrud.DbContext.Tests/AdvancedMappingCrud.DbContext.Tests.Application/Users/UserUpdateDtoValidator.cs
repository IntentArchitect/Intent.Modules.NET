using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public UserUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}