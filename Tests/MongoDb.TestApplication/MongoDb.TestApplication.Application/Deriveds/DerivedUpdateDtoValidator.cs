using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.Deriveds
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DerivedUpdateDtoValidator : AbstractValidator<DerivedUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public DerivedUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.DerivedAttribute)
                .NotNull();

            RuleFor(v => v.BaseAttribute)
                .NotNull();
        }
    }
}