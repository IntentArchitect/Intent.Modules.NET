using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SingleIndexEntityCreateDtoValidator : AbstractValidator<SingleIndexEntityCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public SingleIndexEntityCreateDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.SingleIndex)
                .NotNull();
        }
    }
}