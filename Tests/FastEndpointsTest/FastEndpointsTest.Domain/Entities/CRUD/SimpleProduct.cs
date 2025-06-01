using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class SimpleProduct : IHasDomainEvent
    {
        public SimpleProduct()
        {
            Name = null!;
            Value = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}