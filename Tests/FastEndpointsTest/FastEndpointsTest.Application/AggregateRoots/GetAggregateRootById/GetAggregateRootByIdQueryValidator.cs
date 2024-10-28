using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRootById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootByIdQueryValidator : AbstractValidator<GetAggregateRootByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateRootByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}