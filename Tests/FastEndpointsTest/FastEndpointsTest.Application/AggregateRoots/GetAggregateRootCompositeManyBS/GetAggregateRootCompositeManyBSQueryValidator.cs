using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRootCompositeManyBS
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootCompositeManyBSQueryValidator : AbstractValidator<GetAggregateRootCompositeManyBSQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateRootCompositeManyBSQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}