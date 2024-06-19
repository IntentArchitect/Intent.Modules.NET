using System;
using System.Collections.Generic;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities
{
    public class InLineVo : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Col1 { get; set; }

        public InLineColumns Col2 { get; set; }

        public string Col3 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}