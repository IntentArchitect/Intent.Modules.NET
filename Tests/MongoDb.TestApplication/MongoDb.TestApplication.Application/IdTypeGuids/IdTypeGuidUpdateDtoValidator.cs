using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.IdTypeGuids
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdTypeGuidUpdateDtoValidator : AbstractValidator<IdTypeGuidUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public IdTypeGuidUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}