using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MultikeyIndexEntityDtoValidator : AbstractValidator<MultikeyIndexEntityDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public MultikeyIndexEntityDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.MultiKey)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();
        }
    }
}