using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompoundIndexEntitySingleParentUpdateDtoValidator : AbstractValidator<CompoundIndexEntitySingleParentUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CompoundIndexEntitySingleParentUpdateDtoValidator()
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