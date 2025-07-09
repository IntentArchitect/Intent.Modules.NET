using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Params.GetByIdsQueryTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetByIdsQueryTestHandler : IRequestHandler<GetByIdsQueryTest, int>
    {
        [IntentManaged(Mode.Merge)]
        public GetByIdsQueryTestHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(GetByIdsQueryTest request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetByIdsQueryTestHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}