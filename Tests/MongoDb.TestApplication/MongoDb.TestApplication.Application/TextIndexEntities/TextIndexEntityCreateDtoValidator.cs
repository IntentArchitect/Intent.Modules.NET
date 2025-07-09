using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TextIndexEntityCreateDtoValidator : AbstractValidator<TextIndexEntityCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public TextIndexEntityCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FullText)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();
        }
    }
}