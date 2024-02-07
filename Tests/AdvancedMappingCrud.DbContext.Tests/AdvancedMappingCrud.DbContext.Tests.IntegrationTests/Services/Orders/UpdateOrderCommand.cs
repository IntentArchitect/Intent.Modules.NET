using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Orders
{
    public class UpdateOrderCommand
    {
        public UpdateOrderCommand()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Guid CustomerId { get; set; }

        public static UpdateOrderCommand Create(
            Guid id,
            string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            Guid customerId)
        {
            return new UpdateOrderCommand
            {
                Id = id,
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                CustomerId = customerId
            };
        }
    }
}