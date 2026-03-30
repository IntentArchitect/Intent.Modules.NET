using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetPatternConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPatternConstrainedEntityQueryValidator : AbstractValidator<GetPatternConstrainedEntityQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPatternConstrainedEntityQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}