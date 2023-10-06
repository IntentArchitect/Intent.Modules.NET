using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class StereotypeIndex : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid DefaultIndexField { get; set; }

        public Guid CustomIndexField { get; set; }

        public Guid GroupedIndexFieldA { get; set; }

        public Guid GroupedIndexFieldB { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}