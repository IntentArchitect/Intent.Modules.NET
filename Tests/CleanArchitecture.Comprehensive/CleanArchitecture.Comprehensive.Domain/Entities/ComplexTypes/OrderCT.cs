using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.ComplexTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class OrderCT : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual ICollection<LineItemCT> LineItemCTS { get; set; } = new List<LineItemCT>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}