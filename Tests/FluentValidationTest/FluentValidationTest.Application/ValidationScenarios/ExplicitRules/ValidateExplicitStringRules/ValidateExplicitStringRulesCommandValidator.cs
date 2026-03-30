using System.Text.RegularExpressions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateExplicitStringRules
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidateExplicitStringRulesCommandValidator : AbstractValidator<ValidateExplicitStringRulesCommand>
    {
        private static readonly Regex RegexTextRegex = new Regex(@"^[A-Z]{2}[0-9]{2}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

        [IntentManaged(Mode.Merge)]
        public ValidateExplicitStringRulesCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RequiredText)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty();

            RuleFor(v => v.EqualityText)
                .NotNull()
                .Equal("MATCHME");

            RuleFor(v => v.InequalityText)
                .NotNull()
                .NotEqual("FORBIDDEN");

            RuleFor(v => v.MinLengthText)
                .NotNull()
                .MinimumLength(3);

            RuleFor(v => v.MaxLengthText)
                .NotNull()
                .MaximumLength(10);

            RuleFor(v => v.RegexText)
                .NotNull()
                .Matches(RegexTextRegex);

            RuleFor(v => v.EmailText)
                .NotNull()
                .EmailAddress();

            RuleFor(v => v.MustText)
                .NotNull()
                .MustAsync(ValidateMustTextAsync);

            RuleFor(v => v.CustomText)
                .NotNull()
                .CustomAsync(ValidateCustomTextAsync);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        private async Task<bool> ValidateMustTextAsync(
            ValidateExplicitStringRulesCommand command,
            string value,
            CancellationToken cancellationToken)
        {
            // TODO: Implement ValidateMustTextAsync (ValidateExplicitStringRulesCommandValidator) functionality
            throw new NotImplementedException("Your custom validation rules here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        private async Task ValidateCustomTextAsync(
            string value,
            ValidationContext<ValidateExplicitStringRulesCommand> validationContext,
            CancellationToken cancellationToken)
        {
            // TODO: Implement ValidateCustomTextAsync (ValidateExplicitStringRulesCommandValidator) functionality
            throw new NotImplementedException("Your custom validation rules here...");
        }
    }
}