using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompoundIndexEntitySingleChildDtoValidator : AbstractValidator<CompoundIndexEntitySingleChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public CompoundIndexEntitySingleChildDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CompoundOne)
                .NotNull();

            RuleFor(v => v.CompoundTwo)
                .NotNull();
        }
    }
}