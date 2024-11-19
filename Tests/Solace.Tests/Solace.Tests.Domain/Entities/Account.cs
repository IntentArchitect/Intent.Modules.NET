using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Solace.Tests.Domain.Entities
{
    public class Account : IHasDomainEvent
    {
        public Account()
        {
            Customer = null!;
        }
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}