using BugSnagTest.ServiceHost.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace BugSnagTest.ServiceHost.Scheduled
{
    public class ScheduledCommand : IRequest, ICommand
    {
        public ScheduledCommand()
        {
        }
    }
}