using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Events
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