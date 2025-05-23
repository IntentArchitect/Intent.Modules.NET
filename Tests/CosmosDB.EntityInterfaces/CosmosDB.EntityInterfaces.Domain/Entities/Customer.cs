using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class Customer : ICustomer, IHasDomainEvent
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
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public IList<string>? Tags { get; set; } = [];

        public Address DeliveryAddress { get; set; }

        public Address? BillingAddress { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}