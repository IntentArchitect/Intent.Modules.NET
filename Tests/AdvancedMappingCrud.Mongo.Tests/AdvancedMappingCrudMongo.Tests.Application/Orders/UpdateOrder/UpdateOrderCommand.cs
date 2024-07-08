using System;
using System.Collections.Generic;
using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(string customerId,
            string refNo,
            DateTime orderDate,
            string externalRef,
            string id,
            List<UpdateOrderCommandOrderItemsDto> orderItems)
        {
            CustomerId = customerId;
            RefNo = refNo;
            OrderDate = orderDate;
            ExternalRef = externalRef;
            Id = id;
            OrderItems = orderItems;
        }

        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string ExternalRef { get; set; }
        public string Id { get; set; }
        public List<UpdateOrderCommandOrderItemsDto> OrderItems { get; set; }
    }
}