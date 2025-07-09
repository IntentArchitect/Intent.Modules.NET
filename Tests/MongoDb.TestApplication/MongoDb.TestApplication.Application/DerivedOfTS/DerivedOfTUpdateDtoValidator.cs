using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.DerivedOfTS
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DerivedOfTUpdateDtoValidator : AbstractValidator<DerivedOfTUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public DerivedOfTUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.DerivedAttribute)
                .NotNull();
        }
    }
}