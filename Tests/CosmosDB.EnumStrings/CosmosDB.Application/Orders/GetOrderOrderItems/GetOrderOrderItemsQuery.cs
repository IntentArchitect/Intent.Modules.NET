using System.Collections.Generic;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Orders.GetOrderOrderItems
{
    public class GetOrderOrderItemsQuery : IRequest<List<OrderOrderItemDto>>, IQuery
    {
        public GetOrderOrderItemsQuery(string orderId, string warehouseId)
        {
            OrderId = orderId;
            WarehouseId = warehouseId;
        }

        public string OrderId { get; set; }
        public string WarehouseId { get; set; }
    }
}