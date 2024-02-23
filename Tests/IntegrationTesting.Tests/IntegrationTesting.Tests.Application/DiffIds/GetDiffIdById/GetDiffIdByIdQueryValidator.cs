using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.GetDiffIdById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDiffIdByIdQueryValidator : AbstractValidator<GetDiffIdByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDiffIdByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}