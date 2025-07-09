using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.IdTypeOjectIdStrs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdTypeOjectIdStrUpdateDtoValidator : AbstractValidator<IdTypeOjectIdStrUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public IdTypeOjectIdStrUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}