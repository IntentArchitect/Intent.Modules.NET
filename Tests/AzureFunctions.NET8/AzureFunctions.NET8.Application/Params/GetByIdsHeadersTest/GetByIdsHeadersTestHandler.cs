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
        [IntentManaged(Mode.Ignore)]
        public GetByIdsHeadersTestHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(GetByIdsHeadersTest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}