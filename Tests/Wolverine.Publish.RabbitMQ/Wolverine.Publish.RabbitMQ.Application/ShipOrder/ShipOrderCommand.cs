using Intent.RoslynWeaver.Attributes;
using Wolverine.Publish.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Application.ShipOrder
{
    public class ShipOrderCommand : ICommand
    {
        public ShipOrderCommand()
        {
        }
    }
}