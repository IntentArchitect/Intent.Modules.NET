using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.DerivedOfTS
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DerivedOfTCreateDtoValidator : AbstractValidator<DerivedOfTCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public DerivedOfTCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DerivedAttribute)
                .NotNull();
        }
    }
}