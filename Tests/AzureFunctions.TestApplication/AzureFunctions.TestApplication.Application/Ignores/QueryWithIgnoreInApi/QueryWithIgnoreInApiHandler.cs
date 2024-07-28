using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Ignores.QueryWithIgnoreInApi
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryWithIgnoreInApiHandler : IRequestHandler<QueryWithIgnoreInApi, bool>
    {
        [IntentManaged(Mode.Merge)]
        public QueryWithIgnoreInApiHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<bool> Handle(QueryWithIgnoreInApi request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (QueryWithIgnoreInApiHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}