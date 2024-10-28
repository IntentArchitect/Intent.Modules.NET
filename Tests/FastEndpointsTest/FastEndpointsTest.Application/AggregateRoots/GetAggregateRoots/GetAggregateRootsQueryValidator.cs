using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootsQueryValidator : AbstractValidator<GetAggregateRootsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateRootsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}