using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.PatchOrder
{
    public class PatchOrderCommand : IRequest, ICommand
    {
        public PatchOrderCommand(Guid id,
            string? refNo,
            DateTime? orderDate,
            OrderStatus? orderStatus,
            Guid? customerId,
            PatchOrderCommandBillingAddressDto? billingAddress,
            PatchOrderCommandDeliveryAddressDto? deliveryAddress)
        {
            Id = id;
            RefNo = refNo;
            OrderDate = orderDate;
            OrderStatus = orderStatus;
            CustomerId = customerId;
            BillingAddress = billingAddress;
            DeliveryAddress = deliveryAddress;
        }

        public Guid Id { get; set; }
        public string? RefNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public Guid? CustomerId { get; set; }
        public PatchOrderCommandBillingAddressDto? BillingAddress { get; set; }
        public PatchOrderCommandDeliveryAddressDto? DeliveryAddress { get; set; }
    }
}