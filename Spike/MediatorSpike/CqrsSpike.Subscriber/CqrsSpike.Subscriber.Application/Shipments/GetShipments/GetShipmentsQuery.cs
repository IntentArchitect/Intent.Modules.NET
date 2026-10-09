using Mediator;

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipments
{
    public class GetShipmentsQuery : IQuery<List<ShipmentDto>>
    {
        public GetShipmentsQuery()
        {
        }
    }
}