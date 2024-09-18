using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace CosmosDB.Domain.Events
{
    public class RegionChangedDomainEvent : DomainEvent
    {
        public RegionChangedDomainEvent(Region region)
        {
            Region = region;
        }

        public Region Region { get; }
    }
}