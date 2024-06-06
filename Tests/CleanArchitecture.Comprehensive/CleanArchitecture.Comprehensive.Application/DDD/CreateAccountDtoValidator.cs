using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateAccountDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AccNumber)
                .NotNull();

            RuleFor(v => v.Note)
                .NotNull();
        }
    }
}