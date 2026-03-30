using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetNumericConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetNumericConstrainedEntityQueryValidator : AbstractValidator<GetNumericConstrainedEntityQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNumericConstrainedEntityQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}