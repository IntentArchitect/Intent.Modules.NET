using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand()
        {
            RefNo = null!;
            OrderItems = null!;
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
        }

        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Guid CustomerId { get; set; }
        public List<CreateOrderCommandOrderItemsDto> OrderItems { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }

        public static CreateOrderCommand Create(
            string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            Guid customerId,
            List<CreateOrderCommandOrderItemsDto> orderItems,
            string line1,
            string line2,
            string city,
            string postal)
        {
            return new CreateOrderCommand
            {
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                CustomerId = customerId,
                OrderItems = orderItems,
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal
            };
        }
    }
}