using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.Versioned.TestQueryV2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestQueryV2Handler : IRequestHandler<TestQueryV2, int>
    {
        [IntentManaged(Mode.Merge)]
        public TestQueryV2Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(TestQueryV2 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (TestQueryV2Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}