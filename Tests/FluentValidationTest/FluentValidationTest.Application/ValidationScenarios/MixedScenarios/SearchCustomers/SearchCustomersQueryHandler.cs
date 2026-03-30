using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.SearchCustomers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public SearchCustomersQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (SearchCustomersQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}