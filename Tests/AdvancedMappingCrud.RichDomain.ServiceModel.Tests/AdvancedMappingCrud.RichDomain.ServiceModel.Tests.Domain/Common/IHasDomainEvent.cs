using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.HasDomainEventInterface", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common
{
    public interface IHasDomainEvent
    {
        List<DomainEvent> DomainEvents { get; set; }
    }
}