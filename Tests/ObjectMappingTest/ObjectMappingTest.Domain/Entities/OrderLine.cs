using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMappingTest.Domain.Entities
{
    public class OrderLine
    {
        public OrderLine()
        {
            ProductName = null!;
        }

        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public int Qty { get; set; }

        public string? DiscountCode { get; set; }

        public decimal UnitPrice { get; set; }

        public Guid OrderId { get; set; }
    }
}