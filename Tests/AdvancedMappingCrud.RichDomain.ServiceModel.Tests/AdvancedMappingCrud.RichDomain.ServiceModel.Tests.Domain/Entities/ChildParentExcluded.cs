using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class ChildParentExcluded : IHasDomainEvent
    {
        public ChildParentExcluded(string childName, int parentAge)
        {
            ChildName = childName;
            ParentAge = parentAge;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ChildParentExcluded()
        {
            ChildName = null!;
        }
        public Guid Id { get; private set; }

        public string ChildName { get; private set; }

        public int ParentAge { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}