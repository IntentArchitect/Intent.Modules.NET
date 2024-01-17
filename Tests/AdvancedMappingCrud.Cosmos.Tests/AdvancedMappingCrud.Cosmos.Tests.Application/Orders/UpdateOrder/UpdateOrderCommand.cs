using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(string id,
            string customerId,
            string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            List<UpdateOrderCommandOrderTagsDto> orderTags)
        {
            Id = id;
            CustomerId = customerId;
            RefNo = refNo;
            OrderDate = orderDate;
            OrderStatus = orderStatus;
            OrderTags = orderTags;
        }

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<UpdateOrderCommandOrderTagsDto> OrderTags { get; set; }
    }
}