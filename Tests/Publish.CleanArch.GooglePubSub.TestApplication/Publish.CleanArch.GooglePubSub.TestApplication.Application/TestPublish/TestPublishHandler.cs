using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace Publish.CleanArch.GooglePubSub.TestApplication.Application.TestPublish
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestPublishHandler : IRequestHandler<TestPublish>
    {
        [IntentManaged(Mode.Ignore)]
        public TestPublishHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Unit> Handle(TestPublish request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}