using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals.GetUniqueConVals
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUniqueConValsQueryValidator : AbstractValidator<GetUniqueConValsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUniqueConValsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}