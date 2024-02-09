using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.GetDiffIds
{
    public class GetDiffIdsQueryValidator : AbstractValidator<GetDiffIdsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDiffIdsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}