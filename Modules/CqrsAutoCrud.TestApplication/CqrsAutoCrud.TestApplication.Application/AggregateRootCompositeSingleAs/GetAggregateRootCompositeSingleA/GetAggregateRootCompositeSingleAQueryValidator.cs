using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.GetAggregateRootCompositeSingleAById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootCompositeSingleAQueryValidator : AbstractValidator<GetAggregateRootCompositeSingleA>
    {
        [IntentManaged(Mode.Fully)]
        public GetAggregateRootCompositeSingleAQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}