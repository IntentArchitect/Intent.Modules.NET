using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.GetAccounts
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
        }
    }
}