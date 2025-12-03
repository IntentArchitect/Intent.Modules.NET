using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace CompositeMessageBus.Eventing.Messages
{
    public record MsgMassTEvent : IEvent
    {
        public const string PubsubName = "pubsub";
        public const string TopicName = "CompositeMessageBus.Eventing.Messages.MsgMassTEvent";

        public MsgMassTEvent()
        {
            Message = null!;
        }

        public string Message { get; init; }
        string IEvent.PubsubName => PubsubName;
        string IEvent.TopicName => TopicName;
    }
}