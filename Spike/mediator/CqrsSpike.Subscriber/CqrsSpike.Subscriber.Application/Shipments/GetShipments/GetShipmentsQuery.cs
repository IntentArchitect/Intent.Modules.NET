using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipments
{
    public class GetShipmentsQuery : IQuery<List<ShipmentDto>>
    {
        public GetShipmentsQuery()
        {
        }
    }
}