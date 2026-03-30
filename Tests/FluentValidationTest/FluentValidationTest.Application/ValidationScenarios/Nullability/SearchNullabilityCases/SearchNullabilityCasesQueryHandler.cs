using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability.SearchNullabilityCases
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchNullabilityCasesQueryHandler : IRequestHandler<SearchNullabilityCasesQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public SearchNullabilityCasesQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(SearchNullabilityCasesQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (SearchNullabilityCasesQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}