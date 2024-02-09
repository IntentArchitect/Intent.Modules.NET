using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Common;
using Redis.Om.Repositories.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Redis.Om.Repositories.Domain.Events
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