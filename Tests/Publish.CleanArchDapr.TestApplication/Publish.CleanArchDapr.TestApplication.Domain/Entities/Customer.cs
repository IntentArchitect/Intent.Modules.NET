using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Publish.CleanArchDapr.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Customer : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}