using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Solace.Tests.Application.Customers.GetCustom
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomQueryValidator : AbstractValidator<GetCustomQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}