using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.CreateOrderOrderItem
{
    public class CreateOrderOrderItemCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderOrderItemCommand(Guid orderId, int quantity, decimal amount, int units, Guid productId)
        {
            OrderId = orderId;
            Quantity = quantity;
            Amount = amount;
            Units = units;
            ProductId = productId;
        }

        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public int Units { get; set; }
        public Guid ProductId { get; set; }
    }
}