using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.Validation.Validated
{
    public class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
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

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        private async Task ValidateFieldAsync(
            string value,
            ValidationContext<ValidatedCommand> validationContext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your custom validation rules here...");
            validationContext.AddFailure("Custom failure message");
        }
    }
}