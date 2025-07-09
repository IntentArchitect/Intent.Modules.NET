using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.IdTypeOjectIdStrs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdTypeOjectIdStrCreateDtoValidator : AbstractValidator<IdTypeOjectIdStrCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public IdTypeOjectIdStrCreateDtoValidator()
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