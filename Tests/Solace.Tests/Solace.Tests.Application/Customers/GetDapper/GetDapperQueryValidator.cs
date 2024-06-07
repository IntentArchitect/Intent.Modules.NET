using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Solace.Tests.Application.Customers.GetDapper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDapperQueryValidator : AbstractValidator<GetDapperQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDapperQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}