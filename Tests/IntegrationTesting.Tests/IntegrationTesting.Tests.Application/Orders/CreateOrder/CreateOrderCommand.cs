using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(Guid customerId, string refNo, List<CreateOrderCommandOrderItemsDto> orderItems)
        {
            CustomerId = customerId;
            RefNo = refNo;
            OrderItems = orderItems;
        }

        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }
        public List<CreateOrderCommandOrderItemsDto> OrderItems { get; set; }
    }
}