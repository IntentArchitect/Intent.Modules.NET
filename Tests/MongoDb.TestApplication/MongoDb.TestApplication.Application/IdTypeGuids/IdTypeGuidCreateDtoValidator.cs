using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.IdTypeGuids
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdTypeGuidCreateDtoValidator : AbstractValidator<IdTypeGuidCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public IdTypeGuidCreateDtoValidator()
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