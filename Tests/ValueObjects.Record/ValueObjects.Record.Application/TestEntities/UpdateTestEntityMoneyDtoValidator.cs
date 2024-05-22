using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateTestEntityMoneyDtoValidator : AbstractValidator<UpdateTestEntityMoneyDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateTestEntityMoneyDtoValidator()
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