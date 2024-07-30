using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.DeleteAccount
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
        }
    }
}