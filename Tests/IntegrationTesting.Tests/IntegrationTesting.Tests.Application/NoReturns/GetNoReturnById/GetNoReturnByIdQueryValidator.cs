using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.GetNoReturnById
{
    public class GetNoReturnByIdQueryValidator : AbstractValidator<GetNoReturnByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNoReturnByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}