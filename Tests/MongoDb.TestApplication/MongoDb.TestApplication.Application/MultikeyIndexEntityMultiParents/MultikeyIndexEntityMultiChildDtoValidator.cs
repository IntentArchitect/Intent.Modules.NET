using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MultikeyIndexEntityMultiChildDtoValidator : AbstractValidator<MultikeyIndexEntityMultiChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public MultikeyIndexEntityMultiChildDtoValidator()
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