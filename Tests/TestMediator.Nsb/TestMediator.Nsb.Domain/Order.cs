using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestMediator.Nsb.Domain
{
    public class Order
    {
        public Order()
        {
            CustomerName = null!;
        }

        public Guid Id { get; set; }

        public string CustomerName { get; set; }
    }
}