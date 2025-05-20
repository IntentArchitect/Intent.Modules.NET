using FluentValidation;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.GetAccountById;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Accounts.GetAccountById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAccountByIdQueryValidator : AbstractValidator<GetAccountByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAccountByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}