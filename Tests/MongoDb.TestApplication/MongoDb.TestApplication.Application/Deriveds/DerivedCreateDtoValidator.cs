using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.Deriveds
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DerivedCreateDtoValidator : AbstractValidator<DerivedCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public DerivedCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DerivedAttribute)
                .NotNull();

            RuleFor(v => v.BaseAttribute)
                .NotNull();
        }
    }
}