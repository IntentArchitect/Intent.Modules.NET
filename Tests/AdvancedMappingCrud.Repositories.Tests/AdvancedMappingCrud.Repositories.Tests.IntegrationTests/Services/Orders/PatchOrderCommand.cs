using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class PatchOrderCommand
    {

        public Guid Id { get; set; }
        public string? RefNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public Guid? CustomerId { get; set; }
        public PatchOrderCommandBillingAddressDto? BillingAddress { get; set; }
        public PatchOrderCommandDeliveryAddressDto? DeliveryAddress { get; set; }

        public static PatchOrderCommand Create(
            Guid id,
            string? refNo,
            DateTime? orderDate,
            OrderStatus? orderStatus,
            Guid? customerId,
            PatchOrderCommandBillingAddressDto? billingAddress,
            PatchOrderCommandDeliveryAddressDto? deliveryAddress)
        {
            return new PatchOrderCommand
            {
                Id = id,
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                CustomerId = customerId,
                BillingAddress = billingAddress,
                DeliveryAddress = deliveryAddress
            };
        }
    }
}