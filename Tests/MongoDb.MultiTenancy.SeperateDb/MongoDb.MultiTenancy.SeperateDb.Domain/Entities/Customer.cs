using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MongoDb.MultiTenancy.SeperateDb.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}