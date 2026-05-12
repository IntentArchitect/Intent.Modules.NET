using EventingSubscribers.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EventingSubscribers.Application.ProcessAccountUpgrade
{
    public class ProcessAccountUpgradeCommand : IRequest, ICommand
    {
        public ProcessAccountUpgradeCommand()
        {
        }
    }
}