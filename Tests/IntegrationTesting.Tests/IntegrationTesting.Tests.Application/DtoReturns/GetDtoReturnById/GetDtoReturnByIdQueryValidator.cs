using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.GetDtoReturnById
{
    public class GetDtoReturnByIdQueryValidator : AbstractValidator<GetDtoReturnByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDtoReturnByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}