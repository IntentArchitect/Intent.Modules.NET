using System;
using System.Collections.Generic;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities
{
    public class Basic : IHasDomainEvent
    {
        public Basic()
        {
            Col1 = null!;
            Col2 = null!;
            Col3 = null!;
        }
        public Guid Id { get; set; }

        public string Col1 { get; set; }

        public string Col2 { get; set; }

        public string Col3 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}