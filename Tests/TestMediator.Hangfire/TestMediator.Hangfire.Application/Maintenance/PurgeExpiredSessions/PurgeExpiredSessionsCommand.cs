using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.Hangfire.Application.Maintenance.PurgeExpiredSessions
{
    public class PurgeExpiredSessionsCommand : ICommand
    {
        public PurgeExpiredSessionsCommand()
        {
        }
    }
}