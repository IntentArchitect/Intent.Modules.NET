using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.Versioned.TestCommandV1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandV1Handler : IRequestHandler<TestCommandV1>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandV1Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestCommandV1 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (TestCommandV1Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}