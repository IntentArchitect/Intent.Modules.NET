using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SingleIndexEntityCreateDtoValidator : AbstractValidator<SingleIndexEntityCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntityCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.SingleIndex)
                .NotNull();
        }
    }
}