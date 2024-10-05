using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Hangfire.Tests.Application.Job
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class JobCommandHandler : IRequestHandler<JobCommand>
    {
        [IntentManaged(Mode.Merge)]
        public JobCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(JobCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (JobCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}