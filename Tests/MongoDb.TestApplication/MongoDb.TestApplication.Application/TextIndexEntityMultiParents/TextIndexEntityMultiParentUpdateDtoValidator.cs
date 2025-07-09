using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TextIndexEntityMultiParentUpdateDtoValidator : AbstractValidator<TextIndexEntityMultiParentUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public TextIndexEntityMultiParentUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();
        }
    }
}