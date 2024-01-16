using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        private List<string> _tags = new List<string>();
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string Name { get; private set; }

        public IReadOnlyCollection<string>? Tags
        {
            get => _tags.AsReadOnly();
            private set => _tags = new List<string>(value);
        }

        public Address DeliveryAddress { get; private set; }

        public Address? BillingAddress { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}