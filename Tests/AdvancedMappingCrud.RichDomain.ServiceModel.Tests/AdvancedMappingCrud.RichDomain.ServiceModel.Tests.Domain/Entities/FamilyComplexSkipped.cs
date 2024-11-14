using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class FamilyComplexSkipped : IHasDomainEvent
    {
        public FamilyComplexSkipped(string childName,
            int parentId,
            long grandparentId,
            long greatGrandparentId,
            string greatGrandparentName)
        {
            ChildName = childName;
            ParentId = parentId;
            GrandparentId = grandparentId;
            GreatGrandparentId = greatGrandparentId;
            GreatGrandparentName = greatGrandparentName;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected FamilyComplexSkipped()
        {
            ChildName = null!;
            GreatGrandparentName = null!;
        }
        public Guid Id { get; private set; }

        public string ChildName { get; private set; }

        public int ParentId { get; private set; }

        public long GrandparentId { get; private set; }

        public long GreatGrandparentId { get; private set; }

        public string GreatGrandparentName { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}