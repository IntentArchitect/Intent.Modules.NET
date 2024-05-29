using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexStereotypes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateWithUniqueConstraintIndexStereotypesQueryValidator : AbstractValidator<GetAggregateWithUniqueConstraintIndexStereotypesQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetAggregateWithUniqueConstraintIndexStereotypesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}