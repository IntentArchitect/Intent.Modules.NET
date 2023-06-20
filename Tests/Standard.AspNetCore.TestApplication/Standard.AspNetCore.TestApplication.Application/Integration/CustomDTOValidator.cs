using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Integration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomDTOValidator : AbstractValidator<CustomDTO>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public CustomDTOValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ReferenceNumber)
                .NotNull();

        }
    }
}