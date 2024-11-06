using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class FamilyComplexSkipped : IHasDomainEvent
    {
        public Guid Id { get; private set; }

        public string ChildName { get; private set; }

        public int ParentId { get; private set; }

        public long GrandparentId { get; private set; }

        public long GreatGrandparentId { get; private set; }

        public string GreatGrandparentName { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}