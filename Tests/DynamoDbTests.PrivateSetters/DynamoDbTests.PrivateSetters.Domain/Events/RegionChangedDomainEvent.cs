using DynamoDbTests.PrivateSetters.Domain.Common;
using DynamoDbTests.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Events
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