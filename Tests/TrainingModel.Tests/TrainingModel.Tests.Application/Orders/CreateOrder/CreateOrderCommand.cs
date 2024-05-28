using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(string refNo,
            DateTime orderDate,
            Guid customerId,
            List<CreateOrderCommandOrderItemsDto> orderItems)
        {
            RefNo = refNo;
            OrderDate = orderDate;
            CustomerId = customerId;
            OrderItems = orderItems;
        }

        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid CustomerId { get; set; }
        public List<CreateOrderCommandOrderItemsDto> OrderItems { get; set; }
    }
}