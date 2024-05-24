using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities.CreateTestEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTestEntityCommandValidator : AbstractValidator<CreateTestEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTestEntityCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Amount)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateTestEntityMoneyDto>()!);

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateTestEntityAddressDto>()!);
        }
    }
}