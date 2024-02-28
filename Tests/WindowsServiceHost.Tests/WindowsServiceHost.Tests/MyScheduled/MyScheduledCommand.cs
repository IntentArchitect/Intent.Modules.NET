using Intent.RoslynWeaver.Attributes;
using MediatR;
using WindowsServiceHost.Tests.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace WindowsServiceHost.Tests.MyScheduled
{
    public class MyScheduledCommand : IRequest, ICommand
    {
        public MyScheduledCommand()
        {
        }
    }
}