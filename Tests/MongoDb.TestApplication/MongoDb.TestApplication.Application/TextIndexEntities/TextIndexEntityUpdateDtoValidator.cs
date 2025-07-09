using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TextIndexEntityUpdateDtoValidator : AbstractValidator<TextIndexEntityUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public TextIndexEntityUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.FullText)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();
        }
    }
}