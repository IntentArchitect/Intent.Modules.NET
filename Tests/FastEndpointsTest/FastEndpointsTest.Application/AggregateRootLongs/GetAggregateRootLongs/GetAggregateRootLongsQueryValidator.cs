using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.GetAggregateRootLongs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootLongsQueryValidator : AbstractValidator<GetAggregateRootLongsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAggregateRootLongsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}