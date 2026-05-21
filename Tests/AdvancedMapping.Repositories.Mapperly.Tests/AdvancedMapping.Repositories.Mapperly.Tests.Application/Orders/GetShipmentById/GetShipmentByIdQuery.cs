using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders.GetShipmentById
{
    public class GetShipmentByIdQuery : IRequest<ShipmentDto>, IQuery
    {
        public GetShipmentByIdQuery(Guid orderId, Guid id)
        {
            OrderId = orderId;
            Id = id;
        }

        public Guid OrderId { get; set; }
        public Guid Id { get; set; }
    }
}