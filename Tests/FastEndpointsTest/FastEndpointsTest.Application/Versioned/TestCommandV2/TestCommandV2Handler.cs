using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.Versioned.TestCommandV2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandV2Handler : IRequestHandler<TestCommandV2>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandV2Handler()
        {
        }

        /// <summary>
        /// Command comment
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(TestCommandV2 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (TestCommandV2Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}