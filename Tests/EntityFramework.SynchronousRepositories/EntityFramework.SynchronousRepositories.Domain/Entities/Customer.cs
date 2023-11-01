using System;
using System.Collections.Generic;
using EntityFramework.SynchronousRepositories.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFramework.SynchronousRepositories.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}