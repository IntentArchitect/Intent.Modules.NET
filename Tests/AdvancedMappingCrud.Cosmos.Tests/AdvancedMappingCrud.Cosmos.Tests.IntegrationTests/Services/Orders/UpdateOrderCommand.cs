using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class UpdateOrderCommand
    {
        public UpdateOrderCommand()
        {
            Id = null!;
            CustomerId = null!;
            RefNo = null!;
            OrderTags = null!;
        }

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<UpdateOrderCommandOrderTagsDto> OrderTags { get; set; }

        public static UpdateOrderCommand Create(
            string id,
            string customerId,
            string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            List<UpdateOrderCommandOrderTagsDto> orderTags)
        {
            return new UpdateOrderCommand
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                OrderTags = orderTags
            };
        }
    }
}