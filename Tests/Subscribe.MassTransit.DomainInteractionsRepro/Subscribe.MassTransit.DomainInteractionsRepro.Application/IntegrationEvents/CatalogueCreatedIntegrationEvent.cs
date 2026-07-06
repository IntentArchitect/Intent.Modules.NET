using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Eventing.Messages
{
    public record CatalogueCreatedIntegrationEvent
    {
        public Guid Id { get; init; }
    }
}