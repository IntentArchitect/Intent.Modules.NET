using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.UpdateOrderOrderItem
{
    public class UpdateOrderOrderItemCommand : IRequest, ICommand
    {
        public UpdateOrderOrderItemCommand(string orderId, string id, int quantity, decimal amount, string productId)
        {
            OrderId = orderId;
            Id = id;
            Quantity = quantity;
            Amount = amount;
            ProductId = productId;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
    }
}