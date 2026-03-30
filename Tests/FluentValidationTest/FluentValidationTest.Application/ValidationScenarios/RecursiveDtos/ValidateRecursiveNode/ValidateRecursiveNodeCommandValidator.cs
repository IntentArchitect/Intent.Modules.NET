using FluentValidation;
using FluentValidationTest.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.RecursiveDtos.ValidateRecursiveNode
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidateRecursiveNodeCommandValidator : AbstractValidator<ValidateRecursiveNodeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateRecursiveNodeCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Root)
                .NotNull()
                .NotEmpty()
                .SetValidator(provider.GetValidator<RecursiveNodeDto>()!);
        }
    }
}