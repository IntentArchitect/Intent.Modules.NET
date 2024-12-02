using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        private string? _id;
        public Customer()
        {
            Id = null!;
            Name = null!;
            DeliveryAddress = null!;
        }
        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name { get; set; }

        public IList<string>? Tags { get; set; } = [];

        public Address DeliveryAddress { get; set; }

        public Address? BillingAddress { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}