using CqrsSpike.Publisher.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Orders.MarkOrderShipped
{
    public class MarkOrderShippedCommand : IRequest, ICommand
    {
        public MarkOrderShippedCommand(Guid orderId, Guid shipmentId)
        {
            OrderId = orderId;
            ShipmentId = shipmentId;
        }

        public Guid OrderId { get; set; }
        public Guid ShipmentId { get; set; }
    }
}