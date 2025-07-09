using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SingleIndexEntityUpdateDtoValidator : AbstractValidator<SingleIndexEntityUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntityUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.SingleIndex)
                .NotNull();
        }
    }
}