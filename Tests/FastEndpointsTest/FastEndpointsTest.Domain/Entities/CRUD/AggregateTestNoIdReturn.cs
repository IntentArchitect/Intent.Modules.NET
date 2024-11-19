using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class AggregateTestNoIdReturn : IHasDomainEvent
    {
        public AggregateTestNoIdReturn()
        {
            Attribute = null!;
        }
        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}