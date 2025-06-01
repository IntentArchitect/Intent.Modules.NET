using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Module2.Eventing.Messages
{
    public record AccountCreatedIEEvent
    {
        public AccountCreatedIEEvent()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
    }
}