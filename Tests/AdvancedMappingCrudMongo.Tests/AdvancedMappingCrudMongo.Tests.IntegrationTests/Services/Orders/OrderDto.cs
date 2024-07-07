using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            Id = null!;
            CustomerId = null!;
            RefNo = null!;
            ExternalRef = null!;
            Customer = null!;
            OrderItems = null!;
        }

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string ExternalRef { get; set; }
        public OrderCustomerDto Customer { get; set; }
        public List<OrderOrderItemDto> OrderItems { get; set; }

        public static OrderDto Create(
            string id,
            string customerId,
            string refNo,
            DateTime orderDate,
            string externalRef,
            OrderCustomerDto customer,
            List<OrderOrderItemDto> orderItems)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo,
                OrderDate = orderDate,
                ExternalRef = externalRef,
                Customer = customer,
                OrderItems = orderItems
            };
        }
    }
}