using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Parents.GetParentById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetParentByIdQueryValidator : AbstractValidator<GetParentByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetParentByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}