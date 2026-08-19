using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Lenient.Domain.Entities
{
    public class OrderLine
    {
        public OrderLine()
        {
            ProductName = null!;
        }

        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public Guid OrderId { get; set; }
    }
}