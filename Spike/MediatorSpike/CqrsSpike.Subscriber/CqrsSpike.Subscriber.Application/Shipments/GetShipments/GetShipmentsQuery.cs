using CqrsSpike.Subscriber.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipments
{
    public class GetShipmentsQuery : IRequest<List<ShipmentDto>>, IQuery
    {
        public GetShipmentsQuery()
        {
        }
    }
}