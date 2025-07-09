using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Publish.CleanArchDapr.TestApplication.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Customer()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}