using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.ScalarCollectionReturnType.QueryWithCollectionReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryWithCollectionReturnHandler : IRequestHandler<QueryWithCollectionReturn, List<string>>
    {
        [IntentManaged(Mode.Merge)]
        public QueryWithCollectionReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<string>> Handle(QueryWithCollectionReturn request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (QueryWithCollectionReturnHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}