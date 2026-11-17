using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.NET11.Eventing.Messages
{
    public record TestMessageEvent
    {
        public TestMessageEvent()
        {
            Message = null!;
        }

        public string Message { get; init; }
    }
}