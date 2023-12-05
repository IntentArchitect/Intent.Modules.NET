using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.OperationAndConstructorMapping
{
    public class OpAndCtorMapping2 : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual OpAndCtorMapping1 OpAndCtorMapping1 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}