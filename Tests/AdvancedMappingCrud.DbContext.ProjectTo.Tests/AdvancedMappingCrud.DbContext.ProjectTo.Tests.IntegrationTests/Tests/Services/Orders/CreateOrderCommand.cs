using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests.Services.Orders
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand()
        {
            RefNo = null!;
        }

        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Guid CustomerId { get; set; }
        public EntityStatus EntityStatus { get; set; }

        public static CreateOrderCommand Create(
            string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            Guid customerId,
            EntityStatus entityStatus)
        {
            return new CreateOrderCommand
            {
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                CustomerId = customerId,
                EntityStatus = entityStatus
            };
        }
    }
}