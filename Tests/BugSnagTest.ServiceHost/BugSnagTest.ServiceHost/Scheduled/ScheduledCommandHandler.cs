using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace BugSnagTest.ServiceHost.Scheduled
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ScheduledCommandHandler : IRequestHandler<ScheduledCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ScheduledCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(ScheduledCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}