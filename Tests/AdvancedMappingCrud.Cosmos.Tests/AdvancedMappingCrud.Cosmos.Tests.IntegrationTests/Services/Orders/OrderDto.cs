using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            Id = null!;
            CustomerId = null!;
            RefNo = null!;
            OrderItems = null!;
            OrderTags = null!;
        }

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderOrderItemDto> OrderItems { get; set; }
        public List<OrderOrderTagsDto> OrderTags { get; set; }

        public static OrderDto Create(
            string id,
            string customerId,
            string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            List<OrderOrderItemDto> orderItems,
            List<OrderOrderTagsDto> orderTags)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                OrderItems = orderItems,
                OrderTags = orderTags
            };
        }
    }
}