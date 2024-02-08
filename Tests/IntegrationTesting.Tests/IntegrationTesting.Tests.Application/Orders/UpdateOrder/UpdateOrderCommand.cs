using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(Guid id, Guid customerId, string refNo, List<UpdateOrderCommandOrderItemsDto> orderItems)
        {
            Id = id;
            CustomerId = customerId;
            RefNo = refNo;
            OrderItems = orderItems;
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }
        public List<UpdateOrderCommandOrderItemsDto> OrderItems { get; set; }
    }
}