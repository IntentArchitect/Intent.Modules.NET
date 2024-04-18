using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Eventing.Messages
{
    public record EventStartedEvent
    {
        public EventStartedEvent()
        {
            Message = null!;
        }
        public string Message { get; init; }
    }
}