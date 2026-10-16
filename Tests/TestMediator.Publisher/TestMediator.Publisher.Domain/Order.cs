using Intent.RoslynWeaver.Attributes;
using TestMediator.Publisher.Domain.Common;
using TestMediator.Publisher.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestMediator.Publisher.Domain
{
    public class Order : IHasDomainEvent
    {
        public Order(string customerName, decimal total)
        {
            CustomerName = customerName;
            Total = total;
            DomainEvents.Add(new OrderPlacedDomainEvent());
        }
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Order()
        {
            CustomerName = null!;
        }

        public Guid Id { get; set; }

        public string CustomerName { get; set; }

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}