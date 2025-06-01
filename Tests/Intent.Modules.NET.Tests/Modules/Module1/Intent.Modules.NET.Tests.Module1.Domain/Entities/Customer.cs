using Intent.Modules.NET.Tests.Module1.Domain.Common;
using Intent.Modules.NET.Tests.Module1.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Customer(string name)
        {
            Name = name;
            DomainEvents.Add(new CustomerCreated(customer: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Customer()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}