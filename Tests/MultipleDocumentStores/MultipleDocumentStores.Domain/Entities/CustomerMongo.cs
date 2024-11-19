using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MultipleDocumentStores.Domain.Entities
{
    public class CustomerMongo : IHasDomainEvent
    {
        public CustomerMongo()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}