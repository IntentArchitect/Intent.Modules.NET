using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.DDD
{
    public class Camera : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string IdemiaId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}