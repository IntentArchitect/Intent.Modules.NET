using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RoleUpdateDtoValidator : AbstractValidator<RoleUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public RoleUpdateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Priviledges)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<PriviledgeDto>()!));
        }
    }
}