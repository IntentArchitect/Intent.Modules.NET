using CqrsSpike.Subscriber.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.CreateShipment
{
    public class CreateShipmentCommand : IRequest<Guid>, ICommand
    {
        public CreateShipmentCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}