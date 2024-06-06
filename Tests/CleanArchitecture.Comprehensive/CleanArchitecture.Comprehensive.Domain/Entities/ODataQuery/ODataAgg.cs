using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery
{
    public class ODataAgg : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ODataChild> ODataChildren { get; set; } = new List<ODataChild>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}