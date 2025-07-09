using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserRoleDtoValidator : AbstractValidator<CreateUserRoleDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserRoleDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}