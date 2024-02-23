using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Parents.GetParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetParentsQueryValidator : AbstractValidator<GetParentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetParentsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}