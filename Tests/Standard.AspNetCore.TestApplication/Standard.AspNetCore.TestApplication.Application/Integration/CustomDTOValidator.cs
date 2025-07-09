using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.Integration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomDTOValidator : AbstractValidator<CustomDTO>
    {
        [IntentManaged(Mode.Merge)]
        public CustomDTOValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ReferenceNumber)
                .NotNull();
        }
    }
}