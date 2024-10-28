using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.Unversioned.Test
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestQueryHandler : IRequestHandler<TestQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public TestQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (TestQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}