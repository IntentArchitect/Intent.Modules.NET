using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCheckNewCompChildCruds
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCheckNewCompChildCrudsQueryValidator : AbstractValidator<GetCheckNewCompChildCrudsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCheckNewCompChildCrudsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}