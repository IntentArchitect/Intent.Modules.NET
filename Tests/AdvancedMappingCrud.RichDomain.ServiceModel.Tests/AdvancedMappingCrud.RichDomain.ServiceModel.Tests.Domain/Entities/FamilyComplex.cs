using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class FamilyComplex : IHasDomainEvent
    {
        public FamilyComplex(string childName,
            int parentId,
            string parentName,
            long grandParentId,
            long greatGrandParentId,
            string greatGrandParentName,
            int auntId,
            string auntName)
        {
            ChildName = childName;
            ParentId = parentId;
            ParentName = parentName;
            GrandParentId = grandParentId;
            GreatGrandParentId = greatGrandParentId;
            GreatGrandParentName = greatGrandParentName;
            AuntId = auntId;
            AuntName = auntName;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected FamilyComplex()
        {
            ChildName = null!;
            ParentName = null!;
            GreatGrandParentName = null!;
            AuntName = null!;
        }
        public Guid Id { get; private set; }

        public string ChildName { get; private set; }

        public int ParentId { get; private set; }

        public string ParentName { get; private set; }

        public long GrandParentId { get; private set; }

        public long GreatGrandParentId { get; private set; }

        public string GreatGrandParentName { get; private set; }

        public int AuntId { get; private set; }

        public string AuntName { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}