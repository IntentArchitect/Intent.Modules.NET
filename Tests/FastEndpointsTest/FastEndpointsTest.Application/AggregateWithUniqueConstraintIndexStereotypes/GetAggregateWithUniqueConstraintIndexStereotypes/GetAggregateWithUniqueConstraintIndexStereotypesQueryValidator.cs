using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.GetAggregateWithUniqueConstraintIndexStereotypes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateWithUniqueConstraintIndexStereotypesQueryValidator : AbstractValidator<GetAggregateWithUniqueConstraintIndexStereotypesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateWithUniqueConstraintIndexStereotypesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}