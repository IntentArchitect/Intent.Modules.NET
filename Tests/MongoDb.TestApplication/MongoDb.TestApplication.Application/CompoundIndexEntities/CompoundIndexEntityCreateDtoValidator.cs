using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompoundIndexEntityCreateDtoValidator : AbstractValidator<CompoundIndexEntityCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CompoundIndexEntityCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.CompoundOne)
                .NotNull();

            RuleFor(v => v.CompoundTwo)
                .NotNull();
        }
    }
}