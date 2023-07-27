using CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Eventing.Messages
{
    public static class IntegrationTriggeringCreatedEventExtensions
    {
        public static IntegrationTriggeringCreatedEvent MapToIntegrationTriggeringCreatedEvent(this IntegrationTriggering projectFrom)
        {
            return new IntegrationTriggeringCreatedEvent
            {
                Id = projectFrom.Id,
            };
        }
    }
}