using System;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            Guid customerId,
            EntityStatus entityStatus)
        {
            RefNo = refNo;
            OrderDate = orderDate;
            OrderStatus = orderStatus;
            CustomerId = customerId;
            EntityStatus = entityStatus;
        }

        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Guid CustomerId { get; set; }
        public EntityStatus EntityStatus { get; set; }
    }
}