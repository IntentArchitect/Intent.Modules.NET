using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Params.GetByIdsHeadersTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetByIdsHeadersTestHandler : IRequestHandler<GetByIdsHeadersTest, int>
    {
        [IntentManaged(Mode.Merge)]
        public GetByIdsHeadersTestHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(GetByIdsHeadersTest request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetByIdsHeadersTestHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}