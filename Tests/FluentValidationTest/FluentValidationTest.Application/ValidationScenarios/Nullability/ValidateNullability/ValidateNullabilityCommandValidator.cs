using FluentValidation;
using FluentValidationTest.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability.ValidateNullability
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidateNullabilityCommandValidator : AbstractValidator<ValidateNullabilityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateNullabilityCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.RequiredString)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(v => v.OptionalString)
                .MaximumLength(100);

            RuleFor(v => v.RequiredInt)
                .GreaterThanOrEqualTo(1);

            RuleFor(v => v.OptionalInt)
                .LessThanOrEqualTo(1000);

            RuleFor(v => v.RequiredPayload)
                .NotNull()
                .SetValidator(provider.GetValidator<SimplePayloadDto>()!);

            RuleFor(v => v.OptionalPayload)
                .SetValidator(provider.GetValidator<SimplePayloadDto>()!);
        }
    }
}