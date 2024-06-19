using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Solace.Tests.Application.Accounts.UpdateAccount
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAccountCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}