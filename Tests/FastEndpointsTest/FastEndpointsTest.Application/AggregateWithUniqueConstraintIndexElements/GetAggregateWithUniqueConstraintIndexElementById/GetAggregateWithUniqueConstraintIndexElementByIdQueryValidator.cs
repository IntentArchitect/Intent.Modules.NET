using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexElements.GetAggregateWithUniqueConstraintIndexElementById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateWithUniqueConstraintIndexElementByIdQueryValidator : AbstractValidator<GetAggregateWithUniqueConstraintIndexElementByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateWithUniqueConstraintIndexElementByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}