using System;
using System.Collections.Generic;
using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<string>, ICommand
    {
        public CreateOrderCommand(string customerId,
            string refNo,
            DateTime orderDate,
            string externalRef,
            List<CreateOrderCommandOrderItemsDto> orderItems)
        {
            CustomerId = customerId;
            RefNo = refNo;
            OrderDate = orderDate;
            ExternalRef = externalRef;
            OrderItems = orderItems;
        }

        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string ExternalRef { get; set; }
        public List<CreateOrderCommandOrderItemsDto> OrderItems { get; set; }
    }
}