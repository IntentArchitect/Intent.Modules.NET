using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup
{
    public class ComponentType : IHasDomainEvent
    {
        public ComponentType()
        {
            ComponentName = null!;
        }

        public int ComponentTypeId { get; set; }

        public string ComponentName { get; set; }

        public virtual ICollection<ComponentPropertyGroup> ComponentPropertyGroups { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}