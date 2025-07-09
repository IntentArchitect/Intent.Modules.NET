using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMoneyDtoValidator : AbstractValidator<CreateMoneyDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMoneyDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Currency)
                .NotNull();
        }
    }
}