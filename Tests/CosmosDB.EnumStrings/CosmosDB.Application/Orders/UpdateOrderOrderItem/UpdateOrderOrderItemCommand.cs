using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Orders.UpdateOrderOrderItem
{
    public class UpdateOrderOrderItemCommand : IRequest, ICommand
    {
        public UpdateOrderOrderItemCommand(string orderId,
            string id,
            int quantity,
            string description,
            decimal amount,
            string warehouseId)
        {
            OrderId = orderId;
            Id = id;
            Quantity = quantity;
            Description = description;
            Amount = amount;
            WarehouseId = warehouseId;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string WarehouseId { get; set; }
    }
}