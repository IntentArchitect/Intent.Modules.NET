using Intent.RoslynWeaver.Attributes;
using MediatR;
using Wolverine.Publish.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Application.ShipOrder
{
    public class ShipOrderCommand : IRequest, ICommand
    {
        public ShipOrderCommand()
        {
        }
    }
}