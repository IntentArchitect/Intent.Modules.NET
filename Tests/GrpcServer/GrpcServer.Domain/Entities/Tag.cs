using System;
using System.Collections.Generic;
using GrpcServer.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GrpcServer.Domain.Entities
{
    public class Tag : IHasDomainEvent
    {
        public Tag()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}