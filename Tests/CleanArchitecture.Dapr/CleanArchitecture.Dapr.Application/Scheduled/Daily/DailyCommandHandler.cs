using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.Scheduled.Daily
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DailyCommandHandler : IRequestHandler<DailyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DailyCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(DailyCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (DailyCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}