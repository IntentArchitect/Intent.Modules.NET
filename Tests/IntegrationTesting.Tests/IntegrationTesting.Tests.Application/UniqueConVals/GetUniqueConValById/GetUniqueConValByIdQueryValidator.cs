using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals.GetUniqueConValById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUniqueConValByIdQueryValidator : AbstractValidator<GetUniqueConValByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUniqueConValByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}