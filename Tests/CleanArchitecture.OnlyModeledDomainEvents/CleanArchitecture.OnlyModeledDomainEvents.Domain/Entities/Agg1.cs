using System;
using System.Collections.Generic;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Common;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities
{
    public class Agg1 : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Operation()
        {
            DomainEvents.Add(new Agg1Event());
        }
    }
}