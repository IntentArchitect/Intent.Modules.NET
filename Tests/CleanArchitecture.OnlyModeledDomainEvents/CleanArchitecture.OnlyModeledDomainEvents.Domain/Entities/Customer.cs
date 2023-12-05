using System;
using System.Collections.Generic;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Common;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        private string? _id;

        public Customer()
        {
            DomainEvents.Add(new CustomerCreated());
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}