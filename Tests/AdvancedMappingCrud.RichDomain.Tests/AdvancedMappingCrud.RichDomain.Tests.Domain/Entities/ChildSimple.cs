using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class ChildSimple : IHasDomainEvent
    {
        public ChildSimple(string childName, string parentName)
        {
            ChildName = childName;
            ParentName = parentName;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ChildSimple()
        {
            ChildName = null!;
            ParentName = null!;
        }
        public Guid Id { get; private set; }

        public string ChildName { get; private set; }

        public string ParentName { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}