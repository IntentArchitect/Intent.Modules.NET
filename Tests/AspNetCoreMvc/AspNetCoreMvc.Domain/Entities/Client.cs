using System;
using System.Collections.Generic;
using AspNetCoreMvc.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AspNetCoreMvc.Domain.Entities
{
    public class Client : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}