using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntitySingleParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TextIndexEntitySingleChildDtoValidator : AbstractValidator<TextIndexEntitySingleChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public TextIndexEntitySingleChildDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FullText)
                .NotNull();
        }
    }
}