using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompoundIndexEntityMultiChildDtoValidator : AbstractValidator<CompoundIndexEntityMultiChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public CompoundIndexEntityMultiChildDtoValidator()
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