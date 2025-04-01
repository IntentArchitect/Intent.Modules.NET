using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class ContactSecond : IHasDomainEvent
    {
        public ContactSecond()
        {
            ContactName = null!;
            ContactDetailsSecond = null!;
        }

        public Guid Id { get; set; }

        public string ContactName { get; set; }

        public virtual ContactDetailsSecond ContactDetailsSecond { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}