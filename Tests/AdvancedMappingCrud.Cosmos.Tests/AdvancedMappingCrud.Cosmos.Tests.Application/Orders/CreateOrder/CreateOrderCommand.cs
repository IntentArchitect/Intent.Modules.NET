using System;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<string>, ICommand
    {
        public CreateOrderCommand(string customerId, string refNo, DateTime orderDate, OrderStatus orderStatus)
        {
            CustomerId = customerId;
            RefNo = refNo;
            OrderDate = orderDate;
            OrderStatus = orderStatus;
        }

        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}