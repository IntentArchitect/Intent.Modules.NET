using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(Guid id, string refNo, DateTime orderDate, OrderStatus orderStatus, Guid customerId)
        {
            Id = id;
            RefNo = refNo;
            OrderDate = orderDate;
            OrderStatus = orderStatus;
            CustomerId = customerId;
        }

        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Guid CustomerId { get; set; }
    }
}