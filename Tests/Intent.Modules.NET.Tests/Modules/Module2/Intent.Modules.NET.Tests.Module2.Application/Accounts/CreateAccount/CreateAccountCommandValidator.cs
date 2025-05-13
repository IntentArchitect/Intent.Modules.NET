using FluentValidation;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.CreateAccount;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Accounts.CreateAccount
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
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}