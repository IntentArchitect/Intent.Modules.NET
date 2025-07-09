using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntitySingleParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SingleIndexEntitySingleChildDtoValidator : AbstractValidator<SingleIndexEntitySingleChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntitySingleChildDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SingleIndex)
                .NotNull();
        }
    }
}