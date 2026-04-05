using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup
{
    public class ComponentPropertyGroup
    {
        public ComponentPropertyGroup()
        {
            GroupName = null!;
        }

        public int PropertyGroupId { get; set; }

        public string GroupName { get; set; }

        public int ComponentTypeId { get; set; }

        public virtual ICollection<ComponentTypeProperty> ComponentTypeProperties { get; set; } = [];
    }
}