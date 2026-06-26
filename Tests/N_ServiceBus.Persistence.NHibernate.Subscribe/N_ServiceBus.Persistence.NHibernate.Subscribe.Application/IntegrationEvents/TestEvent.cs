using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace N_ServiceBus.Persistence.NHibernate.Publish.Eventing.Messages
{
    public record TestEvent
    {
        public TestEvent()
        {
            Message = null!;
        }

        public string Message { get; init; }
    }
}