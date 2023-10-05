using System;
using System.Collections.Generic;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Common;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Agg1 : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Operation()
        {
            DomainEvents.Add(new Agg1Event());
        }
    }
}