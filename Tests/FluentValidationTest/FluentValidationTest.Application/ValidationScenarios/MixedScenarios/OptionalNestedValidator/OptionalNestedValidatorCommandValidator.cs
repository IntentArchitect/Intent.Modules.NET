using FluentValidation;
using FluentValidationTest.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.OptionalNestedValidator
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OptionalNestedValidatorCommandValidator : AbstractValidator<OptionalNestedValidatorCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OptionalNestedValidatorCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.OptionalField)
                .SetValidator(provider.GetValidator<OptionalSuppliedDto>()!);
        }
    }
}