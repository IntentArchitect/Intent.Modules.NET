using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Orders.DeleteOrderOrderItem
{
    public class DeleteOrderOrderItemCommand : IRequest, ICommand
    {
        public DeleteOrderOrderItemCommand(string orderId, string id, string warehouseId)
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