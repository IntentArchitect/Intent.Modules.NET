using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Orders.GetOrderOrderItemById
{
    public class GetOrderOrderItemByIdQuery : IRequest<OrderOrderItemDto>, IQuery
    {
        public GetOrderOrderItemByIdQuery(string orderId, string id, string warehouseId)
        {
            OrderId = orderId;
            Id = id;
            WarehouseId = warehouseId;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }
        public string WarehouseId { get; set; }
    }
}