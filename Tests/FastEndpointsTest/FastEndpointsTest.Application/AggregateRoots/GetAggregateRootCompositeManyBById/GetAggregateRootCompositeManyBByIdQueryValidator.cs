using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRootCompositeManyBById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootCompositeManyBByIdQueryValidator : AbstractValidator<GetAggregateRootCompositeManyBByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateRootCompositeManyBByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}