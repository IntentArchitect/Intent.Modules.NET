using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Validation.Validated
{
    public class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public ValidatedCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field)
                .NotNull()
                .CustomAsync(ValidateFieldAsync);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task ValidateFieldAsync(
            string value,
            ValidationContext<ValidatedCommand> validationContext,
            CancellationToken cancellationToken)
        {
            validationContext.AddFailure("Custom failure message");
        }
    }
}