using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.Scheduled.Hourly
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class HourlyCommandHandler : IRequestHandler<HourlyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public HourlyCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(HourlyCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}