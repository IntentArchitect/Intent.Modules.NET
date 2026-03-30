using System.Text.RegularExpressions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerRegistrationDtoValidator : AbstractValidator<CustomerRegistrationDto>
    {
        private static readonly Regex WebsiteUrlRegex = new Regex(@"^https?://.+", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        [IntentManaged(Mode.Merge)]
        public CustomerRegistrationDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FirstName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(v => v.LastName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(v => v.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(v => v.Username)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3);

            RuleFor(v => v.WebsiteUrl)
                .Matches(WebsiteUrlRegex);
        }
    }
}