using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTestEntityAddressDtoValidator : AbstractValidator<CreateTestEntityAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTestEntityAddressDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();

            RuleFor(v => v.Line2)
                .NotNull();

            RuleFor(v => v.City)
                .NotNull();

            RuleFor(v => v.Country)
                .NotNull();

            RuleFor(v => v.AddressType)
                .NotNull()
                .IsInEnum();
        }
    }
}