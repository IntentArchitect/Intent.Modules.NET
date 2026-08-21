using System;
using System.Collections.Generic;
using Dapper.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Dapper.Tests.Domain.Entities
{
    public class OrderLine : IHasDomainEvent
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}