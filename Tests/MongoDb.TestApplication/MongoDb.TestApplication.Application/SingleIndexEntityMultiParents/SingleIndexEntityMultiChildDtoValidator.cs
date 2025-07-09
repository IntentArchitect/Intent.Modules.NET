using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntityMultiParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SingleIndexEntityMultiChildDtoValidator : AbstractValidator<SingleIndexEntityMultiChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntityMultiChildDtoValidator()
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