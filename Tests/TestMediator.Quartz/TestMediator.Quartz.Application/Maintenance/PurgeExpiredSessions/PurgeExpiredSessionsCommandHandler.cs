using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Quartz.Application.Maintenance.PurgeExpiredSessions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PurgeExpiredSessionsCommandHandler : ICommandHandler<PurgeExpiredSessionsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PurgeExpiredSessionsCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async ValueTask<Unit> Handle(PurgeExpiredSessionsCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (PurgeExpiredSessionsCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}