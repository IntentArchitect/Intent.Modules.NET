using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetNullabilityConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetNullabilityConstrainedEntityQueryValidator : AbstractValidator<GetNullabilityConstrainedEntityQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNullabilityConstrainedEntityQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}