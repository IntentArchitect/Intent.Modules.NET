using System;
using System.Collections.Generic;
using EntityFramework.SynchronousRepositories.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFramework.SynchronousRepositories.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string No { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}