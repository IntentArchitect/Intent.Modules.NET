using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Orders.CreateOrderOrderItem
{
    public class CreateOrderOrderItemCommand : IRequest<string>, ICommand
    {
        public CreateOrderOrderItemCommand(string orderId, int quantity, string description, decimal amount, string warehouseId)
        {
            OrderId = orderId;
            Quantity = quantity;
            Description = description;
            Amount = amount;
            WarehouseId = warehouseId;
        }

        public string OrderId { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string WarehouseId { get; set; }
    }
}