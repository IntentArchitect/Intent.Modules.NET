using System;
using Finbuckle.SeparateDatabase.TestApplication.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public UserUpdateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Email)
                .NotNull();

            RuleFor(v => v.Username)
                .NotNull();

            RuleFor(v => v.Roles)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateUserRoleDto>()!));
        }
    }
}