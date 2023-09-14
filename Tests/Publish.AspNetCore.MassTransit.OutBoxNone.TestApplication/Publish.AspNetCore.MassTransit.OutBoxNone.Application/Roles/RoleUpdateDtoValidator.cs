using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RoleUpdateDtoValidator : AbstractValidator<RoleUpdateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public RoleUpdateDtoValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Priviledges)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<PriviledgeDto>>()!));
        }
    }
}