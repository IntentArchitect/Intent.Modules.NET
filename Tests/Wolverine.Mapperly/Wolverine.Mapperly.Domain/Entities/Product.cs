using Intent.RoslynWeaver.Attributes;
using Wolverine.Mapperly.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Wolverine.Mapperly.Domain.Entities
{
    /// <summary>
    /// Simple aggregate used to verify Mapperly DTO projection works alongside Wolverine CQRS handlers.
    /// </summary>
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}