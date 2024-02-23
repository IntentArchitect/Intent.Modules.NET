using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.Validation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidatedCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field)
                .NotNull()
                .CustomAsync(ValidateFieldAsync);

            RuleFor(v => v.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(v => v.CascaseTest)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .MinimumLength(1)
                .CustomAsync(ValidateCascaseTestAsync);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task ValidateFieldAsync(
            string value,
            ValidationContext<ValidatedCommand> validationContext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your custom validation rules here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task ValidateCascaseTestAsync(
            string value,
            ValidationContext<ValidatedCommand> validationContext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your custom validation rules here...");
        }
    }
}