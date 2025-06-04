using AspNetCoreCleanArchitecture.Sample.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Order()
        {
            OrderNo = null!;
            Buyer = null!;
        }

        public Guid Id { get; set; }

        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public Guid BuyerId { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; } = [];

        public virtual Buyer Buyer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}