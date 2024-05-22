using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ValueObjects.Class.Application.TestEntities.UpdateTestEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateTestEntityCommandValidator : AbstractValidator<UpdateTestEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateTestEntityCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Amount)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateTestEntityMoneyDto>()!);

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateTestEntityAddressDto>()!);
        }
    }
}