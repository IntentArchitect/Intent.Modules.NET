using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public UserCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotNull();

            RuleFor(v => v.Username)
                .NotNull();

            RuleFor(v => v.Roles)
                .NotNull();

        }
    }
}