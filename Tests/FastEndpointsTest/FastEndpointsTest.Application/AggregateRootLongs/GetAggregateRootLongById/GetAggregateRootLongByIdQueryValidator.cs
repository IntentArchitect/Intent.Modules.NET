using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.GetAggregateRootLongById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootLongByIdQueryValidator : AbstractValidator<GetAggregateRootLongByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateRootLongByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}