using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class UpdateOrderCommand
    {
        public UpdateOrderCommand()
        {
            CustomerId = null!;
            RefNo = null!;
            ExternalRef = null!;
            Id = null!;
            OrderItems = null!;
        }

        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string ExternalRef { get; set; }
        public string Id { get; set; }
        public List<UpdateOrderCommandOrderItemsDto> OrderItems { get; set; }

        public static UpdateOrderCommand Create(
            string customerId,
            string refNo,
            DateTime orderDate,
            string externalRef,
            string id,
            List<UpdateOrderCommandOrderItemsDto> orderItems)
        {
            return new UpdateOrderCommand
            {
                CustomerId = customerId,
                RefNo = refNo,
                OrderDate = orderDate,
                ExternalRef = externalRef,
                Id = id,
                OrderItems = orderItems
            };
        }
    }
}