using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders.UpdateOrderOrderItem
{
    public class UpdateOrderOrderItemCommand : IRequest, ICommand
    {
        public UpdateOrderOrderItemCommand(Guid orderId, Guid id, string description, Guid productId)
        {
            OrderId = orderId;
            Id = id;
            Description = description;
            ProductId = productId;
        }

        public Guid OrderId { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid ProductId { get; set; }
    }
}