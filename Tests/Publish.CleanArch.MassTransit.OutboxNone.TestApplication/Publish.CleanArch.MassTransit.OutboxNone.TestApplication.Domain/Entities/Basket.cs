using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities
{
    public class Basket : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}