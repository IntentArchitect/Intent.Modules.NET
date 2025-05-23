using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.DeleteAccount
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAccountCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}