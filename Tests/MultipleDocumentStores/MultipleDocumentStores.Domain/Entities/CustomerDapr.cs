using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MultipleDocumentStores.Domain.Entities
{
    public class CustomerDapr : IHasDomainEvent
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}