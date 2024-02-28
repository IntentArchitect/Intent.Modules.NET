using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace WindowsServiceHost.Tests.MyScheduled
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyScheduledCommandHandler : IRequestHandler<MyScheduledCommand>
    {
        [IntentManaged(Mode.Merge)]
        public MyScheduledCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(MyScheduledCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}