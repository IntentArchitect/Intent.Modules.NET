using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class FamilyComplex : IHasDomainEvent
    {
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