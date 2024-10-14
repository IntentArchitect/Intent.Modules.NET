using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Orders
{
    public class OrderCreateDto
    {
        public OrderCreateDto()
        {
            RefNo = null!;
            OrderItems = null!;
        }

        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderCreateOrderOrderItemDto> OrderItems { get; set; }

        public static OrderCreateDto Create(string refNo, DateTime orderDate, List<OrderCreateOrderOrderItemDto> orderItems)
        {
            return new OrderCreateDto
            {
                RefNo = refNo,
                OrderDate = orderDate,
                OrderItems = orderItems
            };
        }
    }
}