using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PreferenceDtoValidator : AbstractValidator<PreferenceDto>
    {
        [IntentManaged(Mode.Merge)]
        public PreferenceDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Key)
                .NotNull();

            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}