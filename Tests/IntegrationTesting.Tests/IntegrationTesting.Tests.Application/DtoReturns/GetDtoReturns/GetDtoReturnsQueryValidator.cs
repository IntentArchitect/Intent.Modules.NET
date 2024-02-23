using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.GetDtoReturns
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDtoReturnsQueryValidator : AbstractValidator<GetDtoReturnsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDtoReturnsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}