using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Solace.Tests.Application.Customers.GetEFSql
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEFSqlQueryValidator : AbstractValidator<GetEFSqlQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEFSqlQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}