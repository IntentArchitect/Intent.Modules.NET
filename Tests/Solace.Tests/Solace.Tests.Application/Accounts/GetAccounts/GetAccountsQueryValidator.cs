using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Solace.Tests.Application.Accounts.GetAccounts
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