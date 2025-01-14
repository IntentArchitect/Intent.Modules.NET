using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCNCCChildren
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCNCCChildrenQueryValidator : AbstractValidator<GetCNCCChildrenQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCNCCChildrenQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}