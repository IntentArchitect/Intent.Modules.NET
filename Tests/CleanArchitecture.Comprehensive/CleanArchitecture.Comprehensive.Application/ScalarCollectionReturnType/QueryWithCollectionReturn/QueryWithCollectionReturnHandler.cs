using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ScalarCollectionReturnType.QueryWithCollectionReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryWithCollectionReturnHandler : IRequestHandler<QueryWithCollectionReturn, List<string>>
    {
        [IntentManaged(Mode.Ignore)]
        public QueryWithCollectionReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<string>> Handle(QueryWithCollectionReturn request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}