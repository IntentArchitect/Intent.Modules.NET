using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Order()
        {
            Number = null!;
        }
        public Guid Id { get; set; }

        public string Number { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}