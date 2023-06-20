using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TextIndexEntityCreateDtoValidator : AbstractValidator<TextIndexEntityCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public TextIndexEntityCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FullText)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();
        }
    }
}