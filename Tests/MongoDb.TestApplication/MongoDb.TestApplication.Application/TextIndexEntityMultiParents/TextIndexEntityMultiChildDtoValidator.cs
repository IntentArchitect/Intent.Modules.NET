using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TextIndexEntityMultiChildDtoValidator : AbstractValidator<TextIndexEntityMultiChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public TextIndexEntityMultiChildDtoValidator()
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