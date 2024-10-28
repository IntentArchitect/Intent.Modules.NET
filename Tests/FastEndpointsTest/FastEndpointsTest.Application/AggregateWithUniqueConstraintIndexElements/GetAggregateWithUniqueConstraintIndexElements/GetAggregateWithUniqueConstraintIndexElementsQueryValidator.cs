using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexElements.GetAggregateWithUniqueConstraintIndexElements
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateWithUniqueConstraintIndexElementsQueryValidator : AbstractValidator<GetAggregateWithUniqueConstraintIndexElementsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateWithUniqueConstraintIndexElementsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}