using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Versioned.TestQueryV2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestQueryV2Handler : IRequestHandler<TestQueryV2, int>
    {
        [IntentManaged(Mode.Ignore)]
        public TestQueryV2Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(TestQueryV2 request, CancellationToken cancellationToken)
        {
            return int.Parse(request.Value) + 5;
        }
    }
}