using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders.CreateOrderOrderItem
{
    public class CreateOrderOrderItemCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderOrderItemCommand(Guid orderId, string description, Guid productId)
        {
            OrderId = orderId;
            Description = description;
            ProductId = productId;
        }

        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public Guid ProductId { get; set; }
    }
}