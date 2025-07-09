using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Unversioned.Test
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestQueryHandler : IRequestHandler<TestQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public TestQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            return int.Parse(request.Value);
        }
    }
}