using System;
using System.Collections.Generic;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}