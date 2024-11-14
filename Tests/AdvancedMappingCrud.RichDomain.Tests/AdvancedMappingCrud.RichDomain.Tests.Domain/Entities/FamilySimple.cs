using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class FamilySimple : IHasDomainEvent
    {
        public FamilySimple(string childName, int parentId, string parentName, string grandparentName, long grandparentId)
        {
            ChildName = childName;
            ParentId = parentId;
            ParentName = parentName;
            GrandparentName = grandparentName;
            GrandparentId = grandparentId;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected FamilySimple()
        {
            ChildName = null!;
            ParentName = null!;
            GrandparentName = null!;
        }
        public Guid Id { get; private set; }

        public string ChildName { get; private set; }

        public int ParentId { get; private set; }

        public string ParentName { get; private set; }

        public long GrandparentId { get; private set; }

        public string GrandparentName { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}