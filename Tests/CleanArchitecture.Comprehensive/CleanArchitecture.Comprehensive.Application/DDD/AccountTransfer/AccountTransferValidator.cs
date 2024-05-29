using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.AccountTransfer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AccountTransferValidator : AbstractValidator<AccountTransfer>
    {
        [IntentManaged(Mode.Merge)]
        public AccountTransferValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();

            RuleFor(v => v.Currency)
                .NotNull();
        }
    }
}