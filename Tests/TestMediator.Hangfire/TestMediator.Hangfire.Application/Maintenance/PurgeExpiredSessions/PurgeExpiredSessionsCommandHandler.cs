using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Hangfire.Application.Maintenance.PurgeExpiredSessions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PurgeExpiredSessionsCommandHandler : ICommandHandler<PurgeExpiredSessionsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PurgeExpiredSessionsCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Unit> Handle(PurgeExpiredSessionsCommand request, CancellationToken cancellationToken)
        {
            // The command models no associations: the job only proves that Hangfire dispatches it through Mediator.
            return Unit.Value;
        }
    }
}
