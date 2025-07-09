using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users
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

            RuleFor(v => v.UserName)
                .NotNull();

            RuleFor(v => v.Preferences)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<PreferenceDto>()!));
        }
    }
}