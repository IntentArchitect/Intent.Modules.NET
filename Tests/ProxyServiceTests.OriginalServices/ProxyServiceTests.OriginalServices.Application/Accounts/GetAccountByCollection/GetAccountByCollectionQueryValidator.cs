using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.GetAccountByCollection
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAccountByCollectionQueryValidator : AbstractValidator<GetAccountByCollectionQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAccountByCollectionQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Collection)
                .NotNull();
        }
    }
}