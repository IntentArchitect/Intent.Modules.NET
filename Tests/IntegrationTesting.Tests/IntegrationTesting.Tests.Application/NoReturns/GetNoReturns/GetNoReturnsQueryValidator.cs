using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.GetNoReturns
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetNoReturnsQueryValidator : AbstractValidator<GetNoReturnsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNoReturnsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}