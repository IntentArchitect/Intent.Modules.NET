using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.ODataQuery
{
    public class ODataAgg : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ODataChild> ODataChildren { get; set; } = new List<ODataChild>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}