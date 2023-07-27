using CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Eventing.Messages
{
    public static class IntegrationTriggeringUpdatedEventExtensions
    {
        public static IntegrationTriggeringUpdatedEvent MapToIntegrationTriggeringUpdatedEvent(this IntegrationTriggering projectFrom)
        {
            return new IntegrationTriggeringUpdatedEvent
            {
                Id = projectFrom.Id,
            };
        }
    }
}