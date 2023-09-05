using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public record DelayedNotificationEvent
    {
        public DelayedNotificationEvent()
        {
            Message = null!;
        }
        public string Message { get; init; }
    }
}