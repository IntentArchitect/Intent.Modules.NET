using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MultikeyIndexEntityUpdateDtoValidator : AbstractValidator<MultikeyIndexEntityUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public MultikeyIndexEntityUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.MultiKey)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();
        }
    }
}