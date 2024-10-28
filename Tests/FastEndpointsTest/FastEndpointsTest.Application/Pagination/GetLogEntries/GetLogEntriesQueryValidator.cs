using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.Pagination.GetLogEntries
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetLogEntriesQueryValidator : AbstractValidator<GetLogEntriesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetLogEntriesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}