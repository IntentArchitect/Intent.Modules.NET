using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.Deriveds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DerivedDtoValidator : AbstractValidator<DerivedDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public DerivedDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
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