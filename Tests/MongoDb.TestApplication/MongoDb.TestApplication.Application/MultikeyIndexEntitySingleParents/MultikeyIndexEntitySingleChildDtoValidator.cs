using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MultikeyIndexEntitySingleChildDtoValidator : AbstractValidator<MultikeyIndexEntitySingleChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public MultikeyIndexEntitySingleChildDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MultiKey)
                .NotNull();
        }
    }
}