using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Solace.Tests.Application.Accounts.CreateAccount
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAccountCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}