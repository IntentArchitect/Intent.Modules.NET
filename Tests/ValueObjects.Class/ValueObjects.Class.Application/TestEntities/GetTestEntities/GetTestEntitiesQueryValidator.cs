using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ValueObjects.Class.Application.TestEntities.GetTestEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetTestEntitiesQueryValidator : AbstractValidator<GetTestEntitiesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetTestEntitiesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}