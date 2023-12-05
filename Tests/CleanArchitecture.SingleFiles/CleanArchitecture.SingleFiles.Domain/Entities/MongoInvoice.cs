using System;
using System.Collections.Generic;
using CleanArchitecture.SingleFiles.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.SingleFiles.Domain.Entities
{
    public class MongoInvoice : IHasDomainEvent
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public ICollection<MongoLine> MongoLines { get; set; } = new List<MongoLine>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}