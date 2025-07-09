using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixQueriesWithType.MyRequest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyRequestQueryHandler : IRequestHandler<MyRequestQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public MyRequestQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(MyRequestQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (MyRequestQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}