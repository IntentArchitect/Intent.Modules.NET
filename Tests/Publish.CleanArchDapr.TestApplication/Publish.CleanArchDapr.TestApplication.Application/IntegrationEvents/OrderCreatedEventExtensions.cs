using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Eventing.Messages
{
    public static class OrderCreatedEventExtensions
    {
        public static OrderCreatedEvent MapToOrderCreatedEvent(this Order projectFrom)
        {
            return new OrderCreatedEvent
            {
                Id = projectFrom.Id,
                CustomerId = projectFrom.CustomerId,
            };
        }
    }
}