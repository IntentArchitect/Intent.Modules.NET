using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTestEntityMoneyDtoValidator : AbstractValidator<CreateTestEntityMoneyDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTestEntityMoneyDtoValidator()
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