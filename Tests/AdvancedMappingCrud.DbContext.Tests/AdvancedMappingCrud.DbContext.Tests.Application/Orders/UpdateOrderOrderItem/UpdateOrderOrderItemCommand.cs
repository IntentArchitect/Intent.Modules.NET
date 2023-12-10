using System;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Orders.UpdateOrderOrderItem
{
    public class UpdateOrderOrderItemCommand : IRequest, ICommand
    {
        public UpdateOrderOrderItemCommand(Guid orderId, Guid id, int quantity, decimal amount, Guid productId)
        {
            OrderId = orderId;
            Id = id;
            Quantity = quantity;
            Amount = amount;
            ProductId = productId;
        }

        public Guid OrderId { get; set; }
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Guid ProductId { get; set; }
    }
}