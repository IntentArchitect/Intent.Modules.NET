using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public partial class BaseClass : IBaseClass, IHasDomainEvent
    {
        public Guid Id { get; protected set; }

        public string BaseAttribute { get; protected set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}