using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.SearchAggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SearchAggregateRootsQueryValidator : AbstractValidator<SearchAggregateRootsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public SearchAggregateRootsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AggregateAttr)
                .NotNull();
        }
    }
}