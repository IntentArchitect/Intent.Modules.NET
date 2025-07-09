using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompoundIndexEntityUpdateDtoValidator : AbstractValidator<CompoundIndexEntityUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CompoundIndexEntityUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.CompoundOne)
                .NotNull();

            RuleFor(v => v.CompoundTwo)
                .NotNull();
        }
    }
}