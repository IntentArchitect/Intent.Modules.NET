using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Wolverine.CQRS.TestApplication.Domain.Entities.Items
{
    /// <summary>
    /// Simple aggregate root used by the CQRS sample handlers.
    /// </summary>
    public class Item : IHasDomainEvent
    {
        public Item()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}