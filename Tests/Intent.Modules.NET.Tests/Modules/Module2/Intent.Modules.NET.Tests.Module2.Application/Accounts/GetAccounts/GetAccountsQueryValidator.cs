using FluentValidation;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.GetAccounts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Accounts.GetAccounts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAccountsQueryValidator : AbstractValidator<GetAccountsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAccountsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}