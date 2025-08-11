using DynamoDbTests.EntityInterfaces.Domain.Common;
using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Events
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